using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocomotionAnimation
{
    public float legsHeight;
    public Vector3 legsOffset;
    public Vector2 moveInput;
	public bool grounded;
    public bool landed = true;
	public bool jumped;
	public bool isWalking;
    public bool isFlattened;
    public bool isFlying;
    public bool isImmerged;

	public System.Action onLegAnimationEnd;
	public Rigidbody rigidbody;
	Transform parentTransform;
    CapsuleCollider legsCollider;
    public LegAnimator legs;
    Transform visualsTransform;
    Transform headDummy;
    bool areLegsRetracted = false;
    float legsGrowSpeed = 10f;
    float walkAnimationSwitchCooldown = 1f;
    float walkAnimationSwitchTimer = 0f;

    Dictionary<float, string> movementAnimationMagnitude = new Dictionary<float, string>();

    public LocomotionAnimation(Rigidbody rb, CapsuleCollider legsCollider, Transform visualsTransform)
    {
        this.rigidbody = rb;
        this.legsCollider = legsCollider;
        parentTransform = legsCollider.transform;
        this.visualsTransform = visualsTransform;
        GrowLegs();
        onLegAnimationEnd += legs.onAnimationEnded;
        DefaultWalkCycle();
    }

    public void DefaultWalkCycle()
    {
        movementAnimationMagnitude.Clear();
        movementAnimationMagnitude.Add(0f, "Idle");
        movementAnimationMagnitude.Add(0.25f, "Run");
    }

    public void HeavyWalkCycle()
    {
        movementAnimationMagnitude.Clear();
        movementAnimationMagnitude.Add(0f, "Idle");
        movementAnimationMagnitude.Add(0.25f, "WalkHeavy");
    }

    public void Update()
    {
        if (Game.i.player.GetCurrentController() == null) return;
        legs.transform.localPosition = legsOffset - Vector3.up*(legsHeight);
        SetLegHeight();

        if(isImmerged)
        {
            legs.gameObject.SetActive(true);
            legs.speed = 1f;
            legs.PlayOnce("Water");
        }
        else if(isFlattened)
		{
            legs.gameObject.SetActive(true);
            legs.speed = 1f;
			legs.PlayOnce("Flat");
        }
        else if(isFlying)
        {
            legs.gameObject.SetActive(true);
            legs.speed = 1f;
            legs.PlayOnce("Fly");
        }
        else if (!grounded)
        {
			if(!jumped)
			{
				jumped = true;
				landed = false;

				this.legs.speed = 10f;
				legs.PlayOnce("Jump", () => {
					this.legs.speed = 1f;
					this.legs.PlayOnce("Air");
				});
			}
		}
        else
        {
			if(jumped)
			{
				jumped = false;

				legs.speed = 1f;
				legs.PlayOnce("Land", () => {
					this.landed = true;
				});
			}
			else if (landed)
			{
				PlayAnimation(new Vector2(rigidbody.velocity.x, rigidbody.velocity.z).magnitude * moveInput.magnitude);
			}
        }

        if (walkAnimationSwitchTimer > 0) walkAnimationSwitchTimer -= Time.deltaTime;
    }

    string previousAnimation = "";
    public void PlayAnimation(float hVelocity)
	{
		string animName = "Idle";
		foreach (KeyValuePair<float, string> entry in movementAnimationMagnitude)
		{
            if (hVelocity > entry.Key)
            {
                if(previousAnimation == "Walk" || previousAnimation == "Run") 
                {
                    if(entry.Value == "Walk" || entry.Value == "Run")
                    {
                        if (walkAnimationSwitchTimer <= 0f) animName = entry.Value;
                    }
                    else animName = entry.Value;
                }
                else animName = entry.Value;
            }
		}
		if (animName != "Idle") legs.speed = Mathf.Clamp(hVelocity/2, 0.5f, 2f);
		else legs.speed = 1f;

        legs.PlayOnce(animName);
        previousAnimation = animName;
    }

    public bool AreLegsRetracted()
    {
        return areLegsRetracted;
    }

    public void RetractLegs()
    {
        legs.gameObject.SetActive(false);
        areLegsRetracted = true;
        legsCollider.enabled = false;
        ResetVisualRotation(); // TODO : Remove
    }

    public void ExtendLegs()
    {
        legs.gameObject.SetActive(true);
        areLegsRetracted = false;
        legsCollider.enabled = true;
        ResetVisualRotation(); // TODO : Remove
        legs.Play("Idle");
    }

    void ResetVisualRotation()
    {
        if(visualsTransform != null) visualsTransform.localEulerAngles = Vector3.zero;
    }

    public GameObject GetEjectionClone()
    {
        var animObject = Object.Instantiate(legs.gameObject, legs.transform.position, legs.transform.rotation).GetComponent<LegAnimator>();
        animObject.transform.parent = null;
        animObject.gameObject.SetActive(true);
        animObject.enabled = true;

        var visuals = Object.Instantiate(visualsTransform.gameObject, visualsTransform.position, visualsTransform.rotation);
        visuals.transform.localScale = Vector3.one;
        animObject.Attach(visuals.transform);
        visuals.transform.localPosition = Vector3.zero;
        visuals.transform.localEulerAngles = Vector3.zero;

		Object.Destroy(animObject.gameObject, 2f);

        return animObject.gameObject;
    }

    void GrowLegs()
    {
        legs = Object.Instantiate(Game.i.library.legsPrefab, parentTransform)
        .GetComponent<LegAnimator>();

        legs.transform.localPosition = legsOffset;
        headDummy = legs.transform.GetChild(0); // Head position, symbolized by an empty object

		/*
		GameObject trigger = new GameObject();
		trigger.transform.SetParent(legs.transform);
		trigger.transform.localPosition = Vector3.zero;
		trigger.name = "GroundedTrigger";
		SphereCollider sc = trigger.AddComponent<SphereCollider>();
		sc.isTrigger = true;
		sc.radius = 0.05f;
		sc.center = new Vector3(0f, -sc.radius, 0f);
		groundedTrigger = trigger.AddComponent<CollisionEventTransmitter>();


		groundedTrigger.onTriggerEnter += (other) =>
		{
			jumped = true;
		};

		groundedTrigger.onTriggerExit += (other) =>
		{
			jumped = false;
		};
		*/
	}

    void SetLegHeight()
    {
        /*
        if (areLegsRetracted) {
            legsCollider.size = new Vector3(0.2f, 0.2f, 0.2f);
            legsCollider.center = Vector3.zero;
        }
        else {
            legsCollider.size = new Vector3(0.2f, Mathf.Lerp(legsCollider.size.y, legsHeight/2f, legsGrowSpeed * Time.deltaTime), 0.2f);
            legsCollider.center = Vector3.Lerp(legsCollider.center, (-legsOffset / 2f), legsGrowSpeed * Time.deltaTime);
        }
		*/
        legsCollider.radius = 0.25f;
        legsCollider.height = legsHeight/2f;
		legsCollider.center = -legsOffset/2f;
	}

    public Geometry.PositionAndRotationAndScale GetHeadDummy()
    {
        return new Geometry.PositionAndRotationAndScale() { position = headDummy.position, rotation = headDummy.rotation, scale = headDummy.localScale };
    }
}
