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
    public float legsHeight = 1f;
    public Vector3 legsOffset;
    public AperturePreset customCamera = null;

    new internal Rigidbody rigidbody;
    internal CapsuleCollider legsCollider;
    internal bool crouching = false;
    internal Legs legs;

    public abstract void OnEject();
    public abstract void OnPossess();

    internal virtual void OnJump() { }
    public void OnToggleCrouch()
    {
        if(canCrouch)
        {
            crouching = !crouching;
            RefreshCrouch();
        }
    }

    private void RefreshCrouch()
    {
        if(crouching)
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
        if(Physics.Raycast(transform.position + legsOffset, -Vector3.up, out hit))
        {
            return hit.point;
        }

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

    public void Move(Vector3 direction)
    {
        if(crouching) SpecificMove(direction);
        else
        {
            Vector3 camdir = new Vector3(Camera.main.transform.forward.x, 0f, Camera.main.transform.forward.z);
            Vector3 dir = new Vector3(camdir.x * direction.x, 0f, camdir.z * direction.z);

            Debug.Log(camdir.x * direction.x);
            transform.transform.position += dir * Time.deltaTime * 5f;
        }
    }

    internal virtual void SpecificMove(Vector3 direction)
    {
        
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
        RefreshCrouch();
    }

    virtual internal void Update()
    {

    }


    virtual internal void FixedUpdate()
    {

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
