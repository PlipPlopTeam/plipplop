using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public abstract class Controller : MonoBehaviour
{
    [Header("Inherited properties")]
    public bool addRigidBody = false;
    public bool autoPossess = false;
    public bool canCrouch = true;

    [Header("Locomotion")]
    public float speed = 3f;
    public float legsHeight = 1f;
    public float jump = 10f;
    public float groundCheckRange = 0.5f;

    public Vector3 legsOffset;
    public AperturePreset customCamera = null;

    [Header("Gravity")]
    public float baseDrag = 15f;
    public float strength = 0.5f;
    public float maxFallSpeed = 10f;
    public float checkGroundDistance;

    new internal Rigidbody rigidbody;
    internal CapsuleCollider legsCollider;
    internal bool isCrouching = false;
    internal Legs legs;

    public virtual void OnEject()
    {
        isCrouching = true;
        RefreshCrouch();
    }
    
    public virtual void OnPossess()
    {
        isCrouching = false;
        RefreshCrouch();
    }

    internal virtual void SpecificJump() {}
    internal virtual void OnJump()
    { 
        if(isCrouching) SpecificJump();
        else
        {
            if(IsGrounded()) 
                rigidbody.velocity = new Vector3(rigidbody.velocity.x, jump, rigidbody.velocity.z);
        }
    }

    bool IsGrounded()
    {
        Debug.DrawRay(transform.position + legsOffset - new Vector3(10f, legsHeight - 0.1f, 0f), Vector3.down, Color.red, 1f);
        return Physics.Raycast(transform.position + legsOffset - new Vector3(0f, legsHeight - 0.1f, 0f), Vector3.down, groundCheckRange);
    }

    public void OnToggleCrouch()
    {
        if(canCrouch)
        {
            isCrouching = !isCrouching;
            RefreshCrouch();
        }
    }

    private void RefreshCrouch()
    {
        if(isCrouching)
        {
            Crouch();
            
            if(!legs) GrowLegs();
            legs.gameObject.SetActive(false);
            legsCollider.enabled = false;
        }
        else
        {
            Stand();

            if(!legs) GrowLegs();
            legs.gameObject.SetActive(true);
            legsCollider.enabled = true;

            Vector3 surfacePosition = GetBelowSurface();
            if(surfacePosition != Vector3.zero)
            {
                transform.position = new Vector3(transform.position.x, surfacePosition.y + legsHeight, transform.position.z);
            }
        }   
    }

    private Vector3 GetBelowSurface()
    {
        RaycastHit hit;
        if(Physics.Raycast(transform.position + legsOffset, -Vector3.up, out hit)) return hit.point;
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

    internal virtual void Crouch() {}
    internal virtual void Stand() {}
    internal virtual void OnHoldJump() { }
    internal virtual void SpecificMove(Vector3 direction) {}

    public void Move(Vector3 direction)
    {
        if(isCrouching) SpecificMove(direction);
        else
        {
            Vector3 clampDirection = Vector3.ClampMagnitude(direction, 1f);
            //Vector3 camdir = Vector3.one;//new Vector3(Camera.main.transform.forward.x, 0f, Camera.main.transform.forward.z);

            //Vector3 dir = new Vector3(clampDirection.x * Game.i.aperture.Right().x,  0f, clampDirection.z  * Game.i.aperture.Right().z);
            Vector3 dir = clampDirection.x * Game.i.aperture.Right() + clampDirection.z * Game.i.aperture.Forward();
            // Add Movement Force
            rigidbody.AddForce(dir * Time.deltaTime * speed, ForceMode.Impulse);

            // Rotate legs
            if(legs != null && dir != Vector3.zero) legs.transform.forward = -dir;            
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
    }

    virtual internal void Start()
    {
        if(autoPossess) Game.i.player.Possess(this);

        if(!IsPossessed()) OnEject();
        else OnPossess();

        RefreshCrouch();
    }

    virtual internal void Update()
    {

    }


    virtual internal void FixedUpdate()
    {
        if(!IsGrounded()) 
        {
            Vector3 v = rigidbody.velocity + Vector3.down * strength;
            if(v.y < -maxFallSpeed) v.y = -maxFallSpeed;
            rigidbody.velocity = v;
        }
    }

    // Draw a gizmo if i'm being possessed
    void OnDrawGizmos()
    {
        if (EditorApplication.isPlaying) {
            if (IsPossessed()) {
                Gizmos.DrawIcon(transform.position + Vector3.up * 2f, "Favorite Icon");
            }
            else {
                Gizmos.DrawIcon(transform.position + Vector3.up * 2f, "d_CollabChangesConflict Icon");
            }
        }
    }
}
