using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Locomotion : Walker
{
    public LocomotionPreset preset;
	private float groundCheckOffset = 0.5f;
	private float groundCheckRange = 1f;
    private float groundCheckDistanceRange = 0.1f;
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
    float jumpTimer = 0f;
    bool canJump = true;

    internal Vector3 groundCheckDirection = Vector3.down;

	public override void ApplyModifier(float value)
	{
		speedMultiplier *= value;
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

        var legsCollider = gameObject.AddComponent<CapsuleCollider>();
        legsCollider.material = new PhysicMaterial() { dynamicFriction = preset.groundFriction, staticFriction = preset.groundFriction, frictionCombine = preset.frictionCombine };
        locomotionAnimation = new LocomotionAnimation(rigidbody, legsCollider, parentController.visuals);
        isInitialized = true;

        onLegAnimationEnd += locomotionAnimation.onLegAnimationEnd;
    }
    
    private void Update()
    {
        rigidbody = parentController.rigidbody;
        locomotionAnimation.rigidbody = rigidbody;

        if (!AreLegsRetracted())
		{
			locomotionAnimation.grounded = IsGrounded(0.75f);
			locomotionAnimation.legsOffset = legsOffset;
            locomotionAnimation.legsHeight = legsHeight;
            locomotionAnimation.Update();
        }
        locomotionAnimation.isFlattened = isFlattened;
        locomotionAnimation.isImmerged = isImmerged;

        if (locomotionAnimation.isFlattened) locomotionAnimation.Update();

        if(!canJump)
        {
            if (jumpTimer > 0) jumpTimer -= Time.deltaTime;
            else
            {
                canJump = true;
            }
        }

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
            float o = v.Value.y + (IsGrounded() ? legsHeight/2f + legsOffset.y/2f : 0f);
            transform.position = new Vector3(transform.position.x, o, transform.position.z);
        }
        else
		{
            Debug.LogWarning("Could not detect the ground surface when expanding legs from " + gameObject.name);
            //transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        }

        SoundPlayer.PlayAtPosition("sfx_pop_legs", transform.position);
    }

    public void Move(Vector3 direction)
    {
		bool grounded = IsGrounded();
		Vector3 currentVelocity = Vector3.zero;

        locomotionAnimation.moveInput = new Vector2(direction.x, direction.z);

        if(rigidbody != null)
        {
            currentVelocity = rigidbody.velocity;
            if(rigidbody.GetHorizontalVelocity().magnitude > 1f && grounded)
                locomotionAnimation.isWalking = true;
            else
                locomotionAnimation.isWalking = false;
        }

        // Brutal half turn
        if (direction.magnitude != 0f && Vector3.Distance(lastDirection, direction) > 0.9f)
        {
            timePressed = 0f;
        }

        // Acceleration curve position
        if (timePressed != direction.magnitude) {
            timePressed += Mathf.Sign(direction.magnitude - timePressed) * Time.fixedDeltaTime;
        }

        float currentMaxSpeedAmount = preset.accelerationCurve.Evaluate(timePressed);
        float currentSpeedToDistribute = currentMaxSpeedAmount * preset.maxSpeed;

        // If the difference between real velocity and planned velocity is too high, it is likely the player is currently flying against a wall
        if (!grounded && currentSpeedToDistribute - currentVelocity.magnitude > 2f) {
            timePressed = 0f;
        }

        float controlAmount = grounded ? 1f : isImmerged ? preset.waterControl : preset.airControl / 100f;

        if(rigidbody != null)
        {
			if(hasJumped && rigidbody.velocity.y < 0f)
			{
				hasJumped = false; // Put HasJumped to false only if not grounded and falling
				rigidbody.velocity -= rigidbody.velocity.y * Vector3.up;
			}

            // Making a virtual stick to avoid the player to stop in the air when stick reaches 0 magnitude
            Vector3 virtualStick = Vector3.Lerp(
                Vector3.Scale(Vector3.one - Vector3.up, Game.i.aperture.GetCameraTransform().InverseTransformDirection(rigidbody.velocity).normalized),
                direction,
				grounded ? 1f : direction.magnitude
            ).normalized;

            float frictionMultiplier = (grounded || isImmerged) ? 1f : 1f-preset.airFriction;

            Vector3 velocity =
                currentSpeedToDistribute * frictionMultiplier * (
                    virtualStick.z * Game.i.aperture.Forward() +
                    virtualStick.x * Game.i.aperture.Right()
                )
				+ Vector3.up * rigidbody.velocity.y
            ;

            // Prevents brutal air stops
            float anyControlAmount = (grounded || isImmerged) ? Mathf.Lerp(controlAmount, controlAmount * direction.magnitude, 1f- preset.groundFriction) : controlAmount * direction.magnitude;
            velocity.x = Mathf.Lerp(rigidbody.velocity.x, velocity.x, anyControlAmount);
            velocity.z = Mathf.Lerp(rigidbody.velocity.z, velocity.z, anyControlAmount);

			// Apply changes
			rigidbody.velocity = new Vector3(velocity.x * speedMultiplier, velocity.y, velocity.z * speedMultiplier);

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
        // This one here is a small fix to avoid double-jump. Tweak the value as necessary
        if(canJump && (isImmerged || !hasJumped) && rigidbody.velocity.y <= 4) 
        {
            rigidbody.AddForce(Vector3.up * preset.jump * (parentController.gravityMultiplier / 100f), ForceMode.Acceleration);
            hasJumped = true;
            SoundPlayer.Play("sfx_jump");
            canJump = false;
            jumpTimer = 0.5f;
        }
    }

    public void StartFly()
    {
        locomotionAnimation.isFlying = true;
    }
    public void EndFly()
    {
        locomotionAnimation.isFlying = false;
    }

    public bool IsGrounded(float range = 1f) // But better 😎
    {
        List<RaycastHit> hits = new List<RaycastHit>();

        hits.AddRange(RaycastAllToGround(1f)); // MIDDLE
        //hits.AddRange(RaycastAllToGround(1.2f, new Vector2(groundCheckDistanceRange, 0f)));
        //hits.AddRange(RaycastAllToGround(1.2f, new Vector2(-groundCheckDistanceRange, 0f)));
        //hits.AddRange(RaycastAllToGround(1.2f, new Vector2(0f, groundCheckDistanceRange))); // RIGHT
        //hits.AddRange(RaycastAllToGround(1.2f, new Vector2(0f, -groundCheckDistanceRange))); // LEFT

        foreach (RaycastHit h in hits)
        {
            if(!transform.IsYourselfCheck(h.transform) && !h.collider.isTrigger)
			{
                if(h.normal.y >= 1 - (preset.maxWalkableSteepness / 100f))
				{
                    return true;
                }
            }
        }
        return false;
    }

	public float GetCheckOffset()
	{
		if (AreLegsRetracted()) return legsOffset.y + groundCheckOffset;
		else return legsOffset.y + legsHeight + groundCheckOffset;
	}

    public List<RaycastHit> RaycastAllToGround(float range = 1f, Vector2 offset = new Vector2())
    {
        Vector3 o = new Vector3(offset.x, GetCheckOffset(), offset.y);
        List <RaycastHit> result = new List<RaycastHit>();

        Vector3 start = transform.position + o;
        Vector3 direction = groundCheckDirection;
        float distance = o.y + groundCheckRange * range;

        RaycastHit[] hits = Physics.RaycastAll(start, direction, distance);
        Debug.DrawRay(start, direction);

        foreach (RaycastHit h in hits) result.Add(h);

        return result;
	}

    public Vector3? GetGroundNormal()
    {
        if (!IsGrounded()) throw new System.Exception("Tried to get ground normal BUT controller is not grounded. Please add a check.");

        List<RaycastHit> hits = RaycastAllToGround();
        foreach (RaycastHit h in hits) {
            if (!transform.IsYourselfCheck(h.transform) && !h.collider.isTrigger) {
                return h.normal;
            }
        }
        return null;
    }

    public Vector3? GetBelowSurface(float multiplier = 1f)
    {
        List<RaycastHit> hits = RaycastAllToGround(multiplier);
        foreach (RaycastHit h in hits)
        {
            if (!transform.IsYourselfCheck(h.transform) && !h.collider.isTrigger) return h.point;
        }
        return null;
    }

    public Geometry.PositionAndRotationAndScale GetHeadDummy()
    {
        return locomotionAnimation.GetHeadDummy();
    }
}
