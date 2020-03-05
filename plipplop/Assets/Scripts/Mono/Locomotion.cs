using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Locomotion : Walker
{
    public LocomotionPreset preset;
    private float groundCheckRange = 0.1f;
	[HideInInspector] public float legsHeight { get { return 1f; } }
	private Vector3 legsOffset = Vector3.up * 0.5f;
	[HideInInspector] public bool isFlattened = false;

    [HideInInspector] new public Rigidbody rigidbody;
    [HideInInspector] public Vector3 targetDirection;
    [HideInInspector] public bool isImmerged = false;
    public event System.Action onLegAnimationEnd;

    LocomotionAnimation locomotionAnimation;
    Controller parentController;
	Vector3 lastDirection = new Vector3();
	float speedMultiplier = 1f;
    float timePressed = 0f;
    bool hasJumped = false;
    bool isInitialized = false;

    internal Vector3 groundCheckDirection = Vector3.down;

	public override void ApplyAdherence(float adherence)
	{
		speedMultiplier = 1f - adherence;
	}

	private void Awake()
    {
        if (!isInitialized) Initialize();
    }

    public void Initialize()
    {
        if (isInitialized) return;
        preset = preset ? preset : Game.i.library.defaultLocomotion;
        parentController = GetComponent<Controller>();
        rigidbody = parentController.rigidbody;

        var legsCollider = gameObject.AddComponent<BoxCollider>();
        legsCollider.material = new PhysicMaterial() { dynamicFriction = preset.groundFriction, staticFriction = preset.groundFriction, frictionCombine = preset.frictionCombine };
        locomotionAnimation = new LocomotionAnimation(rigidbody, legsCollider, parentController.visuals);
        isInitialized = true;

        onLegAnimationEnd += locomotionAnimation.onLegAnimationEnd;
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

        locomotionAnimation.isFlattened = isFlattened;
        if (locomotionAnimation.isFlattened) locomotionAnimation.Update();
    }

	public bool AreLegsRetracted()
    {
        return locomotionAnimation.AreLegsRetracted();
    }

    public GameObject GetEjectionClone()
    {
        return locomotionAnimation.GetEjectionClone();
    }

    public void RetractLegs()
    {
        locomotionAnimation.RetractLegs();
	}
    public void ExtendLegs()
    {
		locomotionAnimation.ExtendLegs();
        var v = GetBelowSurface();

        if (v != null) 
        {
            transform.position = new Vector3(transform.position.x, v.Value.y + (IsGrounded() ? legsHeight/2f : 0f), transform.position.z);
        }
        else
		{
            Debug.LogWarning("Could not detect the ground surface when expanding legs from " + gameObject.name);
            transform.position = new Vector3(transform.position.x, transform.position.y + legsHeight/2f + legsOffset.y, transform.position.z);
        }

        SoundPlayer.PlayAtPosition("sfx_pop_legs", transform.position);
    }

    public void Move(Vector3 direction)
    {
        var currentVelocity = Vector3.zero;
        if(rigidbody != null)
        {
            currentVelocity = rigidbody.velocity;
            if(rigidbody.GetHorizontalVelocity().magnitude > 1f && IsGrounded())
                locomotionAnimation.isWalking = true;
            else
                locomotionAnimation.isWalking = false;
        }

        // Brutal half turn
        if (direction.magnitude != 0f && Vector3.Distance(lastDirection, direction) > 0.9f) {
            timePressed = 0f;
        }

        // Acceleration curve position
        if (timePressed != direction.magnitude) {
            timePressed += Mathf.Sign(direction.magnitude - timePressed) * Time.fixedDeltaTime;
        }

        float currentMaxSpeedAmount = preset.accelerationCurve.Evaluate(timePressed);
        float currentSpeedToDistribute = currentMaxSpeedAmount * preset.maxSpeed;

        // If the difference between real velocity and planned velocity is too high, it is likely the player is currently flying against a wall
        if (!IsGrounded() && currentSpeedToDistribute - currentVelocity.magnitude > 2f) {
            timePressed = 0f;
        }

        float controlAmount = IsGrounded() ? 1f : isImmerged ? preset.waterControl : preset.airControl / 100f;


        if(rigidbody != null)
        {
            // Making a virtual stick to avoid the player to stop in the air when stick reaches 0 magnitude
            Vector3 virtualStick = Vector3.Lerp(
                Vector3.Scale(Vector3.one - Vector3.up, Game.i.aperture.GetCameraTransform().InverseTransformDirection(rigidbody.velocity).normalized),
                direction,
                IsGrounded() ? 1f : direction.magnitude
            ).normalized;

            float frictionMultiplier = (IsGrounded() || isImmerged) ? 1f : 1f-preset.airFriction;

            Vector3 velocity =
                currentSpeedToDistribute * frictionMultiplier * (
                    virtualStick.z * Game.i.aperture.Forward() +
                    virtualStick.x * Game.i.aperture.Right()
                )
				+ Vector3.up * rigidbody.velocity.y
            ;

            // Prevents brutal air stops
            float anyControlAmount = (IsGrounded() || isImmerged) ? Mathf.Lerp(controlAmount, controlAmount * direction.magnitude, 1f- preset.groundFriction) : controlAmount * direction.magnitude;
            velocity.x = Mathf.Lerp(rigidbody.velocity.x, velocity.x, anyControlAmount);
            velocity.z = Mathf.Lerp(rigidbody.velocity.z, velocity.z, anyControlAmount);

            // Apply changes
            rigidbody.velocity = velocity * speedMultiplier;

            // Save last direction
            lastDirection = direction;

            // Animation
            if (rigidbody.velocity.normalized.magnitude > 0 && direction.magnitude > 0f) {
                transform.forward = Vector3.Lerp(
                    Vector3.Scale(Vector3.one - Vector3.up, transform.forward),
                    Vector3.Scale(Vector3.one - Vector3.up, rigidbody.velocity.normalized),
                    Time.fixedDeltaTime * preset.lookForwardSpeed
                );
            }
        }
    }

    public void Jump()
    {
        //                                   v   This one here is a small fix to avoid double-jump. Tweak the value as necessary
        if((isImmerged || !hasJumped) && rigidbody.velocity.y <= 4) 
        {
            rigidbody.AddForce(Vector3.up * preset.jump * (parentController.gravityMultiplier / 100f), ForceMode.Acceleration);
            hasJumped = true;
            SoundPlayer.Play("sfx_jump");
        }
    }

    public bool IsGrounded(float rangeMultiplier = 1f) // But better 😎
    {
        RaycastHit[] hits = RaycastAllToGround(rangeMultiplier);
        foreach(RaycastHit h in hits)
        {
            if (!transform.IsYourselfCheck(h.transform) && !h.collider.isTrigger) {
                if (h.normal.y >= 1 - (preset.maxWalkableSteepness / 100f)) {
                    if (hasJumped && rigidbody != null && rigidbody.velocity.y < 0f) {
                        hasJumped = false; // Put HasJumped to false only if not grounded and falling
                        rigidbody.velocity -= rigidbody.velocity.y * Vector3.up;
                    }
                    return true;
                }
            }
        }
        return false;
    }

    public RaycastHit[] RaycastAllToGround(float rangeMultiplier = 1f)
    {
        Vector3 os = Vector3.zero;

        if (AreLegsRetracted())
            os = legsOffset + new Vector3(0f, 1f, 0f);
        else
            os = legsOffset - new Vector3(0f, legsHeight - 1f, 0f);

        return Physics.RaycastAll(transform.position + transform.TransformDirection(os), groundCheckDirection, 1f + groundCheckRange * rangeMultiplier);
	}

    public Vector3? GetGroundNormal()
    {
        if (!IsGrounded()) throw new System.Exception("Tried to get ground normal BUT controller is not grounded. Please add a check.");

        RaycastHit[] hits = RaycastAllToGround();
        foreach (RaycastHit h in hits) {
            if (!transform.IsYourselfCheck(h.transform) && !h.collider.isTrigger) {
                return h.normal;
            }
        }
        return null;
    }

    public Vector3? GetBelowSurface()
    {
        RaycastHit[] hits = RaycastAllToGround();

        foreach (RaycastHit h in hits) {
            if (!transform.IsYourselfCheck(h.transform) && !h.collider.isTrigger) return h.point;
        }

        return null;
    }

    public Geometry.PositionAndRotationAndScale GetHeadDummy()
    {
        return locomotionAnimation.GetHeadDummy();
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
