using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocomotionAnimation
{
    public float legsHeight;
    public Vector3 legsOffset;
    public bool isJumping;
    public bool isWalking;

    float tiltAmplitude = 12f;
    float tiltLerpSpeed = 4f;
    float tilt = 0f;
    Transform parentTransform;
    public Rigidbody rigidbody;
    BoxCollider legsCollider;
    MeshAnimator legs;
    Transform visualsTransform;

    public LocomotionAnimation(Rigidbody rb, BoxCollider legsCollider, Transform visualsTransform)
    {
        this.rigidbody = rb;
        this.legsCollider = legsCollider;
        parentTransform = legsCollider.transform;
        this.visualsTransform = visualsTransform;
        GrowLegs();
    }

    public void Update()
    {
      //  legs.isJumping = isJumping;
      //  legs.velocity = rigidbody.velocity;       v  This will have to be removed when the legs pivot is fixed
        legs.transform.localPosition = legsOffset - Vector3.up*(legsHeight);
        SetLegHeight();

        var tiltDirection = Mathf.Clamp(rigidbody.velocity.magnitude/2f, 0f, 1f) * ((Mathf.Floor(Time.time * 2.6f) % 2) * 2f - 1f); // Will give -1 or 1

        if (isWalking) legs.PlayOnce("Walk");
        else legs.PlayOnce("Idle");

        tilt = Mathf.Lerp(tilt, tiltDirection, Time.deltaTime * tiltLerpSpeed);

        if (isJumping) tilt = 0f;

        if(visualsTransform != null)
        {
            visualsTransform.localEulerAngles = 
            new Vector3(tiltAmplitude * Mathf.Clamp(- rigidbody.velocity.y/5f, -1f, 1f),
                        visualsTransform.localEulerAngles.y,
                        tiltAmplitude * tilt
            );
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

    void GrowLegs()
    {
        legs = Object.Instantiate(Game.i.library.legsPrefab, parentTransform)
        .GetComponent<MeshAnimator>();
     //   legs.body = parentTransform;
        legs.transform.localPosition = legsOffset;
     //   foreach (Leg l in legs.legs) l.maxFootDistance = legsHeight + 2f;
    }

    void SetLegHeight()
    {
        legsCollider.size = new Vector3(1f, legsHeight, 1f);
        legsCollider.center = legsOffset + new Vector3(0f, -legsHeight / 2, 0f);
        legs.transform.localScale = (Vector3.one - Vector3.up) + Vector3.up * legsHeight;
    }
}
