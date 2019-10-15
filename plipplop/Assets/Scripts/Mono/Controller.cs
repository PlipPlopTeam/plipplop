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

    public AperturePreset customCamera = null;
    public Locomotion locomotion;

    new internal Rigidbody rigidbody;

    ControllerSensor controllerSensor;

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
            rigidbody.velocity = new Vector3(rigidbody.velocity.x, locomotion.jump, rigidbody.velocity.z);
        }
    }

    internal void RetractLegs()
    {
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

    public void Move(Vector3 direction)
    {
        if (AreLegsRetracted()) SpecificMove(direction);
        else locomotion.Move(direction);
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

    }

    virtual internal void Update()
    {

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
        locomotion.Fall();
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
