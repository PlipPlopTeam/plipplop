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
    [HideInInspector] public bool isImmerged = false;

    LocomotionAnimation locomotionAnimation;
    Controller parentController;
    float timePressed = 0f;
    float lookForwardSpeed = 8f;
    Vector3 lastDirection = new Vector3();

    internal Vector3 groundCheckDirection = Vector3.down;

    private void Awake()
    {

        preset = preset ? preset : Game.i.library.defaultLocomotion;
        parentController = GetComponent<Controller>();
        rigidbody = parentController.rigidbody;

        var legsCollider = gameObject.AddComponent<BoxCollider>();
        legsCollider.material = new PhysicMaterial() { dynamicFriction = preset.groundFriction, staticFriction = preset.groundFriction, frictionCombine = preset.frictionCombine};
        locomotionAnimation = new LocomotionAnimation(rigidbody, legsCollider, parentController.visuals);
    }
    
    private void Update()
    {
        rigidbody = parentController.rigidbody;
        locomotionAnimation.rigidbody = rigidbody;

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

        Vector3 sp = Vector3.zero;
        var v = GetBelowSurface();

        if (v != null) 
        {
            sp = (Vector3)v;
            transform.position = new Vector3(transform.position.x, sp.y + legsHeight + 0.1f, transform.position.z);
        }
    }

    public void Move(Vector3 direction)
    {
        var currentVelocity = rigidbody.velocity;

        if (rigidbody.GetHorizontalVelocity().magnitude > 1f) {
            locomotionAnimation.isWalking = true;
        }
        else {
            locomotionAnimation.isWalking = false;
        }
        
        // Brutal half turn
        if (direction.magnitude != 0f && Vector3.Distance(lastDirection, direction) > 0.9f)
            timePressed = 0f;

        // Acceleration curve position
        if (timePressed != direction.magnitude)
            timePressed += Mathf.Sign(direction.magnitude - timePressed) * Time.fixedDeltaTime;

        float currentMaxSpeedAmount = preset.accelerationCurve.Evaluate(timePressed);
        float currentSpeedToDistribute = currentMaxSpeedAmount * preset.maxSpeed;

        // If the difference between real velocity and planned velocity is too high, it is likely the player is currently flying against a wall
        if (currentSpeedToDistribute - currentVelocity.magnitude > 2f) {
            timePressed = 0f;
        }

        float controlAmount = IsGrounded() ? 1f : isImmerged ? preset.waterControl : preset.airControl / 100f;

        // Making a virtual stick to avoid the player to stop in the air when stick reaches 0 magnitude
        Vector3 virtualStick = Vector3.Lerp(
            Vector3.Scale(Vector3.one - Vector3.up, Game.i.aperture.cam.transform.InverseTransformDirection(rigidbody.velocity).normalized),
            direction,
            IsGrounded() ? 1f : direction.magnitude
        ).normalized;

        Vector3 velocity =
            currentSpeedToDistribute * (
                virtualStick.z * Game.i.aperture.Forward() +
                virtualStick.x * Game.i.aperture.Right()
            )
            + Vector3.up * rigidbody.velocity.y
        ;

        // Prevents brutal air stops
        float anyControlAmount = (IsGrounded() || isImmerged) ? controlAmount : controlAmount * direction.magnitude;
        velocity.x = Mathf.Lerp(rigidbody.velocity.x, velocity.x, anyControlAmount);
        velocity.z = Mathf.Lerp(rigidbody.velocity.z, velocity.z, anyControlAmount);

        // Apply changes
        rigidbody.velocity = velocity;

        // Save last direction
        lastDirection = direction;

        // Animation
        if ((isImmerged || IsGrounded()) && rigidbody.velocity.normalized.magnitude > 0) {
            transform.forward = Vector3.Lerp(
                Vector3.Scale(Vector3.one - Vector3.up, transform.forward),
                Vector3.Scale(Vector3.one - Vector3.up, rigidbody.velocity.normalized),
                Time.fixedDeltaTime * lookForwardSpeed
            );
        }
    }

    public void Jump()
    {
        rigidbody.AddForce(Vector3.up * preset.jump * (parentController.gravityMultiplier/100f), ForceMode.Acceleration);
    }

    public bool IsGrounded(float rangeMultiplier = 1f) // But better 😎
    {
        RaycastHit[] hits = RaycatAllToGround(rangeMultiplier);

        foreach(RaycastHit h in hits)
        {
            if (!IsMe(h.transform) && !h.collider.isTrigger) {
                if (h.normal.y >= 1-(preset.maxWalkableSteepness/100f))
                    return true;
            }
        }
        return false;
    }

    bool IsMe(Transform thing)
    {
        foreach(Transform t in transform.GetComponentsInChildren<Transform>())
        {
            if(thing == t.transform) return true;
        }
        return false;
    }

    RaycastHit[] RaycatAllToGround(float rangeMultiplier = 1f)
    {
        Vector3 os = Vector3.zero;

        if (AreLegsRetracted())
            os = legsOffset + new Vector3(0f, 0.2f, 0f);
        else
            os = legsOffset - new Vector3(0f, legsHeight - 0.2f, 0f);

        return Physics.RaycastAll(transform.position + transform.TransformDirection(os), groundCheckDirection, groundCheckRange * rangeMultiplier);
    }

    Vector3? GetBelowSurface()
    {
        RaycastHit[] hits = RaycatAllToGround();

        foreach (RaycastHit h in hits) {
            if (!IsMe(h.transform) && !h.collider.isTrigger) return h.point;
        }

        return null;
    }

#if UNITY_EDITOR
    // Draw a gizmo if i'm being possessed
    void OnDrawGizmos()
    {
        if (EditorApplication.isPlaying) {
            // Legs
            Gizmos.color = Color.red;
            if (AreLegsRetracted())
                Gizmos.DrawLine(transform.position + transform.TransformDirection(legsOffset - new Vector3(0f, 0.1f, 0f)), transform.position + transform.TransformDirection(legsOffset - new Vector3(0f, 0.1f, 0f)) + groundCheckDirection * groundCheckRange);
            //else
                //Gizmos.DrawLine(transform.position + legsOffset - new Vector3(0f, legsHeight, 0f), transform.position + legsOffset - new Vector3(0f, legsHeight, 0f) + (-transform.up * groundCheckRange));

        }
    }
#endif
}
