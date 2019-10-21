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
    LocomotionAnimation locomotionAnimation;
    float timePressed = 0f;
    float timeFalling = 0f;
    Vector3 heading = new Vector3();

    private void Awake()
    {
        locomotionAnimation = new LocomotionAnimation(gameObject.AddComponent<CapsuleCollider>());
        preset = preset ? preset : Game.i.library.defaultLocomotion;
        rigidbody = GetComponent<Controller>().rigidbody;
    }
    
    private void Update()
    {
        if (!AreLegsRetracted()) {
            locomotionAnimation.isJumping = !IsGrounded();
            locomotionAnimation.legsOffset = legsOffset;
            locomotionAnimation.legsHeight = legsHeight;
            locomotionAnimation.Update();
        }
    }

    public bool AreLegsRetracted()
    {
        return locomotionAnimation.AreLegsRetracted();;
    }

    public void RetractLegs()
    {
        locomotionAnimation.RetractLegs();
    }
    public void ExtendLegs()
    {
        locomotionAnimation.ExtendLegs();

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
        if (IsGrounded()) {
            timePressed += Time.deltaTime;
            timePressed *= direction.magnitude;
            heading = direction;
        }
        else {
            heading = Vector3.Lerp(heading, direction, preset.airControl / 100f);
        }
        var acceleration = preset.accelerationCurve.Evaluate(timePressed);

        currentSpeed = Mathf.Clamp(preset.maxSpeed * acceleration, 0f, preset.maxSpeed);

        Vector3 dir = heading.x * Game.i.aperture.Right() + heading.z * Game.i.aperture.Forward();
        // Add Movement Force
        rigidbody.AddForce(dir * Time.deltaTime * currentSpeed);

        // Rotate legs
        if (dir != Vector3.zero) targetDirection = dir;

        if (IsGrounded()) {
            transform.forward = Vector3.Lerp(transform.forward, targetDirection, Time.deltaTime * 10f);
        }
        else {

        }
    }

    public void Fall(float factor=1f)
    {
        if (!IsGrounded() && !AreLegsRetracted()) {
            // This code contains few constants to make gravity feel good
            // Feel free to edit but DO NOT make public
            timeFalling += Time.deltaTime*1.6f;
            rigidbody.AddForce(Vector3.down * Mathf.Pow(9.81f, timeFalling+2.8f) * factor * Time.deltaTime, ForceMode.Acceleration);
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
            if(!IsMe(h.transform) && !h.collider.isTrigger) return h.point;
        }

        return null;
    }

    RaycastHit[] RaycatAllToGround(float rangeMultiplier = 1f)
    {
        Vector3 os = Vector3.zero;

        if(AreLegsRetracted())
            os = legsOffset + new Vector3(0f, 0.2f, 0f);
        else
            os = legsOffset - new Vector3(0f, legsHeight - 0.2f, 0f);

        return Physics.RaycastAll(transform.position + transform.TransformDirection(os), Vector3.down, groundCheckRange * rangeMultiplier);
    }

    public bool IsGrounded(float rangeMultiplier = 1f) // But better 😎
    {
        RaycastHit[] hits = RaycatAllToGround(rangeMultiplier);

        foreach(RaycastHit h in hits)
        {
            if (!IsMe(h.transform) && !h.collider.isTrigger) {
                timeFalling = 0f;
                return true;
            }
        }
        return false;
    }

    private bool IsMe(Transform thing)
    {
        foreach(Transform t in transform.GetComponentsInChildren<Transform>())
        {
            if(thing == t.transform) return true;
        }
        return false;
    }

    public void Jump()
    {
        rigidbody.AddForce(Vector3.up * preset.jump, ForceMode.Acceleration);
    }

#if UNITY_EDITOR
    // Draw a gizmo if i'm being possessed
    void OnDrawGizmos()
    {
        if (EditorApplication.isPlaying) {
            // Legs
            Gizmos.color = Color.red;
            if (AreLegsRetracted())
                Gizmos.DrawLine(transform.position + transform.TransformDirection(legsOffset - new Vector3(0f, 0.1f, 0f)), transform.position + transform.TransformDirection(legsOffset - new Vector3(0f, 0.1f, 0f)) + Vector3.down*groundCheckRange);
            //else
                //Gizmos.DrawLine(transform.position + legsOffset - new Vector3(0f, legsHeight, 0f), transform.position + legsOffset - new Vector3(0f, legsHeight, 0f) + (-transform.up * groundCheckRange));

        }
    }
#endif
}
