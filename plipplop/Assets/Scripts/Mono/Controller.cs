using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public abstract class Controller : MonoBehaviour
{
    [Header("AUTO POSSESS")]
    public bool autoPossess = false;
    [Header("Inherited properties")]
    public bool beginCrouched = false;
    public bool canRetractLegs = true;
    public bool useGravity = true;
    [Range(1f, 200f)] public float gravityMultiplier = 8f;

    public AperturePreset customCamera = null;
    [HideInInspector] public Locomotion locomotion;

    new internal Rigidbody rigidbody;
    internal ControllerSensor controllerSensor;

    public virtual void OnEject()
    {
        if (controllerSensor) Destroy(controllerSensor.gameObject);
        controllerSensor = null;

        RetractLegs();

        //DEBUG
        foreach (var renderer in GetComponentsInChildren<Renderer>()) {
            renderer.material.color = new Color(70 / 255f, 100 / 255f, 160 / 255f);
        }
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
    }

    internal virtual void SpecificJump() {}
    internal virtual void OnJump()
    { 
        if(AreLegsRetracted()) 
            SpecificJump();
        else if (IsGrounded()) {
            locomotion.Jump();
        }
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

    internal virtual bool IsGrounded() { return locomotion.IsGrounded(); }

    internal virtual void OnHoldJump() { }
    internal abstract void OnLegsRetracted();
    internal abstract void OnLegsExtended();
    internal virtual void SpecificMove(Vector3 direction) {}

    virtual internal void BaseMove(Vector3 direction)
    {
        if (AreLegsRetracted()) SpecificMove(direction);
        else locomotion.Move(direction);
    }


    public void Move(float fb, float rl)
    {
        BaseMove(new Vector3(rl, 0f, fb).normalized);
    }

    internal bool IsPossessed()
    {
        return Game.i.player.IsPossessing(this);
    }

    virtual internal void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
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

    virtual internal void Update()
    {
        rigidbody.useGravity = false;

        // DEBUG
        var lr = GetComponent<LineRenderer>();
        if (controllerSensor) {
            if (!lr) lr = gameObject.AddComponent<LineRenderer>();
            lr.material = new Material(Shader.Find("Lightweight Render Pipeline/Particles/Unlit"));
            if (controllerSensor.IsThereAnyController()) {
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
        if (useGravity && AreLegsRetracted()) ApplyGravity();
        locomotion.Fall();
    }

    internal void ApplyGravity(float factor=1f)
    {                                    // 9.81 is earth's gravity
        rigidbody.AddForce(Vector3.down * 9.81f * gravityMultiplier * factor * Time.fixedDeltaTime, ForceMode.Acceleration);
    }

    // Trying to possess something else
    virtual internal void OnTryPossess()
    {
        if (controllerSensor && controllerSensor.IsThereAnyController())
        {
            Game.i.player.Possess(controllerSensor.GetFocusedController());
        }
        else
        {
            Game.i.player.PossessBaseController();
        }
    }

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
                })
            );
        }
    }
}
