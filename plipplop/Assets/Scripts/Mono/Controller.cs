using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
[System.Serializable]
public abstract class Controller : MonoBehaviour
{
    [HideInInspector] public bool autoPossess = false;
    [HideInInspector] public bool beginCrouched = false;
    [HideInInspector] public bool canRetractLegs = true;
    [HideInInspector] public bool useGravity = true;
    [HideInInspector] public float gravityMultiplier = 100f;

    [HideInInspector] public Rigidbody customExternalRigidbody;

    [HideInInspector] public AperturePreset customCamera = null;
    [HideInInspector] public Locomotion locomotion;
    
    GameObject face;

    new internal Rigidbody rigidbody;
    internal ControllerSensor controllerSensor;
    internal bool isImmerged;

    public virtual void OnEject()
    {
        if (controllerSensor) Destroy(controllerSensor.gameObject);
        controllerSensor = null;

        RetractLegs();

        //DEBUG
        foreach (var renderer in GetComponentsInChildren<Renderer>())
        {
            renderer.material.color = new Color(70 / 255f, 100 / 255f, 160 / 255f);
        }

        ToggleFace(false);
    }
    
    public virtual void OnPossess()
    {
        controllerSensor = Instantiate(Game.i.library.controllerSensor, gameObject.transform).GetComponent<ControllerSensor>();
        controllerSensor.transform.localPosition = new Vector3(0f, 0f, controllerSensor.sensorForwardPosition);

        if (beginCrouched) {
            RetractLegs();
        }
        else {
            ExtendLegs();
        }

        //DEBUG
        foreach (var renderer in GetComponentsInChildren<Renderer>()) {
            renderer.material.color = new Color(140 / 255f, 60 / 255f, 60 / 255f);
        }

        ToggleFace(true);
    }

    internal virtual void SpecificJump() {}
    internal virtual void OnJump()
    { 
        if(AreLegsRetracted()) 
            SpecificJump();
        else if (IsGrounded())
            locomotion.Jump();
    }

    internal void RetractLegs()
    {
        if (!canRetractLegs) return;
        locomotion.RetractLegs();
        OnLegsRetracted();
    }

    internal void ExtendLegs()
    {
        locomotion.ExtendLegs();
        OnLegsExtended();
    }

    internal bool AreLegsRetracted() { return locomotion.AreLegsRetracted(); }

    internal virtual bool IsGrounded(float rangeMultiplier = 1f) { return locomotion.IsGrounded(rangeMultiplier); }

    internal virtual void OnHoldJump() { }
    internal abstract void OnLegsRetracted();
    internal abstract void OnLegsExtended();
    internal virtual void SpecificMove(Vector3 direction) { }
    internal virtual void MoveCamera(Vector2 d) { }

    virtual internal void BaseMove(Vector3 direction)
    {
        if (AreLegsRetracted()) SpecificMove(direction);
        else locomotion.Move(direction);
    }


    public void Move(float fb, float rl)
    {
        BaseMove(new Vector3(rl, 0f, fb).normalized);
    }

    public void MoveCamera(float h, float v)
    {
        MoveCamera(new Vector2(h, v));
    }
    internal bool IsPossessed()
    {
        return Game.i.player.IsPossessing(this);
    }

    public void SetUnderwater()
    {
        isImmerged = true;
        locomotion.isImmerged = isImmerged;
        if (!Game.i.player.IsPossessingBaseController()) {
            Game.i.player.TeleportBaseControllerAndPossess();
        }
    }

    public void SetOverwater()
    {
        isImmerged = false;
        locomotion.isImmerged = isImmerged;
    }

    virtual internal void Awake()
    {
        rigidbody = customExternalRigidbody==null ? GetComponent<Rigidbody>() : customExternalRigidbody;
        if (!rigidbody) rigidbody = gameObject.AddComponent<Rigidbody>();

        locomotion = GetComponent<Locomotion>();
        if (!locomotion) locomotion = gameObject.AddComponent<Locomotion>();

        //DEBUG
        foreach (var renderer in GetComponentsInChildren<Renderer>()) {
            renderer.material = Instantiate(renderer.material);
        }
    }

    virtual internal void Start()
    {
        if (autoPossess) Game.i.player.Possess(this);

        if (!IsPossessed()) RetractLegs();

        rigidbody.useGravity = false;
    }

    void AddFace()
    {
        face = Instantiate(Game.i.library.facePrefab, transform);
        Vector3 bounds = GetComponent<Collider>().bounds.size;
        face.transform.localPosition = new Vector3(0f, bounds.y/2, bounds.z/2);
        face.transform.forward = transform.transform.forward;
    }

    void ToggleFace(bool isActive)
    {
        if(!face) AddFace();
        face.SetActive(isActive);
    }

    virtual internal void Update()
    {
        rigidbody.useGravity = false;

        // DEBUG
        var lr = GetComponent<LineRenderer>();
        if (controllerSensor) {
            if (!lr) lr = gameObject.AddComponent<LineRenderer>();
            lr.material = Game.i.library.lineRendererMaterial;//new Material(Shader.Find("Lightweight Render Pipeline/Particles/Unlit"));
            if (!isImmerged && controllerSensor.IsThereAnyController()) {
                lr.positionCount = 2;
                lr.SetPosition(0, gameObject.transform.position);
                lr.SetPosition(1, controllerSensor.GetFocusedController().transform.position);
                lr.startColor = Color.red;
                lr.endColor = Color.blue;
                lr.startWidth = 0.1f;
                lr.endWidth = 0.1f;
            }
            else {
                Destroy(lr);
            }
        }
        else {
            if (lr) Destroy(lr);
        }
    }


    virtual internal void FixedUpdate()
    {
        if (useGravity && !isImmerged) {
            if (AreLegsRetracted()) ApplyGravity();
            locomotion.Fall();
        }
        else {
            locomotion.ResetTimeFalling();
        }
    }

    internal void ApplyGravity(float factor=1f)
    {
        rigidbody.AddForce(Vector3.down * 9.81f * gravityMultiplier * factor * Time.fixedDeltaTime, ForceMode.Acceleration);
    }

    // Trying to possess something else
    virtual internal void OnTryPossess()
    {
        if (!isImmerged && controllerSensor && controllerSensor.IsThereAnyController())
        {
            var focused = controllerSensor.GetFocusedController();
            if (!focused.isImmerged) {
                Game.i.player.Possess(focused);
            }
        }
        else if (!Game.i.player.IsPossessingBaseController())
        {
            Game.i.player.TeleportBaseControllerAndPossess();
        }
    }

#if UNITY_EDITOR
    // Draw a gizmo if i'm being possessed
    void OnDrawGizmos()
    {
        if (EditorApplication.isPlaying) {
            // Possession
            if (IsPossessed()) {
                Gizmos.DrawIcon(transform.position + Vector3.up * 2f, "Favorite Icon");
            }
            else {
                Gizmos.DrawIcon(transform.position + Vector3.up * 2f, "d_CollabChangesConflict Icon");
            }
          
            Handles.Label(transform.position + Vector3.up*2f, string.Join("\n", new string[] {
                    string.Format("Grounded? {0}", IsGrounded()),
                    string.Format("Retracted? {0}", AreLegsRetracted()),
                    string.Format("Immerged? {0}", isImmerged)
                })
            );
        }
    }
#endif
}
