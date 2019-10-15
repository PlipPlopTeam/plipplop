using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Locomotion : MonoBehaviour
{
    public LocomotionPreset preset;
    public float legsHeight = 1f;
    public float groundCheckRange = 1f;

    public Vector3 legsOffset;

    [HideInInspector] new public Rigidbody rigidbody;
    [HideInInspector] public Vector3 targetDirection;

    float currentSpeed = 0f;
    CapsuleCollider legsCollider;
    Legs legs;

    private void Awake()
    {
        legsCollider = gameObject.AddComponent<CapsuleCollider>();
        legsCollider.material = new PhysicMaterial() { dynamicFriction = 0f, staticFriction = 0f, frictionCombine = PhysicMaterialCombine.Minimum };
        preset = preset ? preset : Game.i.defaultLocomotion;
        rigidbody = GetComponent<Rigidbody>();
    }
    
    private void Update()
    {
        legsCollider.height = legsHeight;
        legsCollider.center = legsOffset + new Vector3(0f, -legsHeight / 2, 0f);

    }

    public bool AreLegsRetracted()
    {
        return legs == null || !legs.gameObject.activeSelf;
    }

    private void GrowLegs()
    {
        legs = Instantiate(Game.i.library.legsPrefab, transform)
        .GetComponent<Legs>();
        legs.body = transform;
        legs.transform.localPosition = legsOffset;
        foreach (Leg l in legs.legs) l.maxFootDistance = legsHeight + 1f;
    }

    public void RetractLegs()
    {
        if (!legs) GrowLegs();

        legs.gameObject.SetActive(false);
        legsCollider.enabled = false;
    }
    public void ExtendLegs()
    {
        if (!legs) GrowLegs();
        legs.gameObject.SetActive(true);
        legsCollider.enabled = true;
        rigidbody.drag = preset.baseDrag;

        Vector3 sp = Vector3.zero;

        var v = GetBelowSurface();
        if(v != null) 
        {
            sp = (Vector3)v;
            transform.position = new Vector3(transform.position.x, sp.y + legsHeight + 0.1f, transform.position.z);
        }
    }

    public void Move(Vector3 direction)
    {
        currentSpeed = Mathf.Clamp(currentSpeed + preset.acceleration * Time.deltaTime, 0f, preset.speed * direction.magnitude);

        Vector3 clampDirection = Vector3.ClampMagnitude(direction, 1f);
        Vector3 dir = clampDirection.x * Game.i.aperture.Right() + clampDirection.z * Game.i.aperture.Forward();

        // Add Movement Force
        rigidbody.AddForce(dir * Time.deltaTime * currentSpeed);

        // Rotate legs
        if (dir != Vector3.zero) targetDirection = dir;

        transform.forward = Vector3.Lerp(transform.forward, targetDirection, Time.deltaTime * 10f);

        legs.velocity = rigidbody.velocity;
    }

    public void Fall(float factor=1f)
    {
        if (!IsGrounded() && !AreLegsRetracted()) {
            rigidbody.AddForce(Vector3.down * preset.strength * factor * Time.deltaTime);
            if (rigidbody.velocity.y < -preset.maxFallSpeed) {
                rigidbody.velocity = new Vector3(rigidbody.velocity.x, -preset.maxFallSpeed, rigidbody.velocity.z);
            }
        }
    }

    private Vector3? GetBelowSurface()
    {
        RaycastHit[] hits = RaycatAllToGround();

        foreach(RaycastHit h in hits)
        {
            foreach(Transform t in transform.GetComponentsInChildren<Transform>())
            {
                if(t != h.transform) return h.point;
            }
        }

        return null;
    }

    RaycastHit[] RaycatAllToGround()
    {
        Vector3 os = Vector3.zero;

        if(AreLegsRetracted())
            os = legsOffset + new Vector3(0f, 0.1f, 0f);
        else
            os = legsOffset - new Vector3(0f, legsHeight - 0.1f, 0f);

        Debug.DrawRay(transform.position + transform.TransformDirection(os), Vector3.down, Color.blue, 1f);
        return Physics.RaycastAll(transform.position + transform.TransformDirection(os), Vector3.down, groundCheckRange);
    }

    public bool IsGrounded() // But better 😎
    {
        RaycastHit[] hits = RaycatAllToGround();

        foreach(RaycastHit h in hits)
        {
            foreach(Transform t in transform.GetComponentsInChildren<Transform>())
            {
                if(t != h.transform) return true;
            }
        }
        
        return false;
    }

    public void Jump()
    {
        rigidbody.AddForce(Vector3.up * preset.jump, ForceMode.Force);
    }

    // Draw a gizmo if i'm being possessed
    void OnDrawGizmos()
    {
        if (EditorApplication.isPlaying) {
            // Legs
            Gizmos.color = Color.red;
            if (AreLegsRetracted())
                Gizmos.DrawLine(transform.position + transform.TransformDirection(legsOffset - new Vector3(0f, 0.1f, 0f)), transform.position + Vector3.down*groundCheckRange);
            //else
                //Gizmos.DrawLine(transform.position + legsOffset - new Vector3(0f, legsHeight, 0f), transform.position + legsOffset - new Vector3(0f, legsHeight, 0f) + (-transform.up * groundCheckRange));

        }
    }
}
