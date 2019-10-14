using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public abstract class Controller : MonoBehaviour
{
    [Header("Inherited properties")]
    public bool addRigidBody = false;
    public bool autoPossess = false;
    public bool keepCrouchState = false;

    [Header("Locomotion")]
    public LocomotionPreset customLocomotion;
    public float legsHeight = 1f;
    public float jump = 10f;
    public float groundCheckRange = 1f;

    public Vector3 legsOffset;
    public AperturePreset customCamera = null;

    new internal Rigidbody rigidbody;
    internal CapsuleCollider legsCollider;
    internal Legs legs;
    internal ControllerSensor controllerSensor;
    internal Vector3 targetDirection;
    internal LocomotionPreset locomotion;

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
    
    public virtual void OnPossess(bool keepCrouched=false)
    {
        controllerSensor = Instantiate(Game.i.library.controllerSensor, gameObject.transform).GetComponent<ControllerSensor>();
        controllerSensor.transform.localPosition = new Vector3(0f, 0f, controllerSensor.sensorForwardPosition);

        if (keepCrouchState && keepCrouched) {
            ExtendLegs();
        }
        else {
            RetractLegs();
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
            rigidbody.velocity = new Vector3(rigidbody.velocity.x, jump, rigidbody.velocity.z);
        }            
    }

    internal virtual bool IsGrounded()
    {
        return 
            AreLegsRetracted() ?
                Physics.Raycast(transform.position + legsOffset, -transform.up, groundCheckRange) :
                                                                                            // Magic 0.1f so the raycast can start above ground and not inside ground
                Physics.Raycast(transform.position + legsOffset - new Vector3(0f, legsHeight + 0.1f, 0f), -transform.up, groundCheckRange);
    }

    public bool AreLegsRetracted()
    {
        return legs == null || !legs.gameObject.activeSelf;
    }

    private Vector3 GetBelowSurface()
    {
        RaycastHit hit;
        if(Physics.Raycast(transform.position + legsOffset + new Vector3(0f, -legsHeight, 0f), Vector3.down, out hit)) return hit.point;
        return Vector3.zero;
    }

    private void GrowLegs()
    {
        legs = Instantiate(Game.i.library.legsPrefab, transform)
        .GetComponent<Legs>();
        legs.body = transform;
        legs.transform.localPosition = legsOffset;
        foreach(Leg l in legs.legs) l.maxFootDistance = legsHeight + 1f;
    }

    internal void RetractLegs() {
        if (!legs) GrowLegs();
        legs.gameObject.SetActive(false);
        legsCollider.enabled = false;
        OnLegsRetracted();
    }
    internal void ExtendLegs()
    {
        if (!legs) GrowLegs();
        legs.gameObject.SetActive(true);
        legsCollider.enabled = true;

        Vector3 surfacePosition = GetBelowSurface();
        if (surfacePosition != Vector3.zero) {
            transform.position = new Vector3(transform.position.x, surfacePosition.y + legsHeight, transform.position.z);
        }

        OnLegsExtended();
    }
    internal virtual void OnHoldJump() { }
    internal abstract void OnLegsRetracted();
    internal abstract void OnLegsExtended();
    internal virtual void SpecificMove(Vector3 direction) {}

    public void Move(Vector3 direction)
    {
        if(AreLegsRetracted()) SpecificMove(direction);
        else
        {
            Vector3 clampDirection = Vector3.ClampMagnitude(direction, 1f);
            //Vector3 camdir = Vector3.one;//new Vector3(Camera.main.transform.forward.x, 0f, Camera.main.transform.forward.z);

            //Vector3 dir = new Vector3(clampDirection.x * Game.i.aperture.Right().x,  0f, clampDirection.z  * Game.i.aperture.Right().z);
            Vector3 dir = clampDirection.x * Game.i.aperture.Right() + clampDirection.z * Game.i.aperture.Forward();
            // Add Movement Force
            rigidbody.AddForce(dir * Time.deltaTime * locomotion.speed, ForceMode.Impulse);

            // Rotate legs
            if(dir != Vector3.zero) targetDirection = dir;
        }
    }


    public void Move(float fb, float rl)
    {
        Move(new Vector3(rl, 0f, fb));
    }

    internal bool IsPossessed()
    {
        return Game.i.player.IsPossessing(this);
    }

    virtual internal void Awake()
    {
        if(addRigidBody) rigidbody = gameObject.AddComponent<Rigidbody>();
        legsCollider = gameObject.AddComponent<CapsuleCollider>();
        legsCollider.height = legsHeight;
        legsCollider.center = legsOffset + new Vector3(0f, -legsHeight/2, 0f);

        //DEBUG
        foreach (var renderer in GetComponentsInChildren<Renderer>()) {
            renderer.material = Instantiate(renderer.material);
        }
    }

    virtual internal void Start()
    {
        if(autoPossess) Game.i.player.Possess(this);

        locomotion = customLocomotion ? customLocomotion : Game.i.defaultLocomotion;

    }

    virtual internal void Update()
    {
        transform.forward = Vector3.Lerp(transform.forward, targetDirection, Time.deltaTime * 5f);

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
        if(rigidbody != null && !IsGrounded()) 
        {
            Vector3 v = rigidbody.velocity + Vector3.down * locomotion.strength;
            if(v.y < -locomotion.maxFallSpeed) v.y = -locomotion.maxFallSpeed;
            rigidbody.velocity = v;
        }
    }

    // Trying to possess something else
    internal void OnTryPossess()
    {
        if (controllerSensor && controllerSensor.IsThereAnyController()) {
            Game.i.player.Possess(controllerSensor.GetFocusedController());
        }
    }

    // Draw a gizmo if i'm being possessed
    void OnDrawGizmos()
    {
        if (EditorApplication.isPlaying) {

            // Legs
            Gizmos.color = Color.red;
            if (AreLegsRetracted())
                Gizmos.DrawLine(transform.position + legsOffset, transform.position + legsOffset + (-transform.up * groundCheckRange));
            else
                Gizmos.DrawLine(transform.position + legsOffset - new Vector3(0f, legsHeight, 0f), transform.position + legsOffset - new Vector3(0f, legsHeight, 0f) + (-transform.up * groundCheckRange));

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
