using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocomotionAnimation
{
    public float legsHeight;
    public Vector3 legsOffset;
    public bool isJumping;
    public bool isWalking;
    public System.Action onLegAnimationEnd;

    Transform parentTransform;
    public Rigidbody rigidbody;
    BoxCollider legsCollider;
    LegAnimator legs;
    Transform visualsTransform;
    Transform headDummy;

    public LocomotionAnimation(Rigidbody rb, BoxCollider legsCollider, Transform visualsTransform)
    {
        this.rigidbody = rb;
        this.legsCollider = legsCollider;
        parentTransform = legsCollider.transform;
        this.visualsTransform = visualsTransform;
        GrowLegs();

        onLegAnimationEnd += legs.onAnimationEnded;
    }

    public void Update()
    {
        if (Game.i.player.GetCurrentController() == null) return;

        legs.transform.localPosition = legsOffset - Vector3.up*(legsHeight);
        SetLegHeight();


        if (isJumping)
        {
            legs.PlayOnce("Jump");
        }
        else
        {
            if (isWalking) legs.PlayOnce("Walk");
            else legs.PlayOnce("Idle");
        }
    }

    public bool AreLegsRetracted()
    {
        return legs == null || !legs.gameObject.activeSelf;
    }

    public void RetractLegs()
    {
        legs.gameObject.SetActive(false);
        legsCollider.enabled = false;
        ResetVisualRotation(); // TODO : Remove
    }

    public void ExtendLegs()
    {
        legs.gameObject.SetActive(true);
        legsCollider.enabled = true;
        ResetVisualRotation(); // TODO : Remove
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
        legsCollider.size = new Vector3(0.2f, legsHeight, 0.2f);
        legsCollider.center = legsOffset + new Vector3(0f, -legsHeight / 2, 0f);
        legs.transform.localScale = (Vector3.one - Vector3.up) + Vector3.up * legsHeight;
    }

    public Geometry.PositionAndRotationAndScale GetHeadDummy()
    {
        return new Geometry.PositionAndRotationAndScale() { position = headDummy.position, rotation = headDummy.rotation, scale = headDummy.localScale };
    }
}
