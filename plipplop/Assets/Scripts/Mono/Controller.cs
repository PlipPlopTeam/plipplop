﻿using System.Collections;
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
    
    new internal Rigidbody rigidbody;
    new internal CapsuleCollider collider;
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
            collider.enabled = false;
        }
        else
        {
            Stand();

            if(!legs) GrowLegs();
            legs.gameObject.SetActive(true);
            collider.enabled = true;

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
        collider = gameObject.AddComponent<CapsuleCollider>();
        collider.height = legsHeight;
        collider.center = legsOffset + new Vector3(0f, -legsHeight/2, 0f);
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
