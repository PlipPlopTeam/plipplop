using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocomotionAnimation
{
    public float legsHeight;
    public Vector3 legsOffset;
    public bool isJumping;
    public bool isWalking;
    public bool isFlattened;
    public System.Action onLegAnimationEnd;

	public Rigidbody rigidbody;
	Transform parentTransform;
    BoxCollider legsCollider;
    LegAnimator legs;
    Transform visualsTransform;
    Transform headDummy;
    bool areLegsRetracted = false;
    float legsGrowSpeed = 10f;

	Dictionary<float, string> movementAnimationMagnitude = new Dictionary<float, string>();

    public LocomotionAnimation(Rigidbody rb, BoxCollider legsCollider, Transform visualsTransform)
    {
        this.rigidbody = rb;
        this.legsCollider = legsCollider;
        parentTransform = legsCollider.transform;
        this.visualsTransform = visualsTransform;
        GrowLegs();

        onLegAnimationEnd += legs.onAnimationEnded;

		movementAnimationMagnitude.Add(0f, "Walk");
		movementAnimationMagnitude.Add(4f, "Run");
	}

	public void Update()
    {
        if (Game.i.player.GetCurrentController() == null) return;

        legs.transform.localPosition = legsOffset - Vector3.up*(legsHeight);
        SetLegHeight();


        if (isFlattened)
		{
            legs.gameObject.SetActive(true);
            legs.PlayOnce("Flat");
        }
        else if (isJumping)
        {
            legs.PlayOnce("Jump");
        }
        else
        {
			if (isWalking)
			{
				PlayAnimation(new Vector2(rigidbody.velocity.x, rigidbody.velocity.z).magnitude);
			}
			else
			{
				legs.PlayOnce("Idle");
			}
        }
    }

	public void PlayAnimation(float hVelocity)
	{
		string animName = "Walk";
		foreach (KeyValuePair<float, string> entry in movementAnimationMagnitude)
		{
			if(hVelocity > entry.Key) animName = entry.Value;
		}
		legs.PlayOnce(animName);
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

		legsCollider.size = new Vector3(0.2f, legsHeight/2f, 0.2f);
		legsCollider.center = -legsOffset/2f;
	}

    public Geometry.PositionAndRotationAndScale GetHeadDummy()
    {
        return new Geometry.PositionAndRotationAndScale() { position = headDummy.position, rotation = headDummy.rotation, scale = headDummy.localScale };
    }
}
