using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocomotionAnimation
{
    public float legsHeight;
    public Vector3 legsOffset;
    public bool isJumping;

    float tiltAmplitude = 12f;
    float tiltLerpSpeed = 4f;
    float tilt = 0f;
    Transform parentTransform;
    public Rigidbody rigidbody;
    BoxCollider legsCollider;
    Legs legs;
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
        legs.isJumping = isJumping;
        legs.velocity = rigidbody.velocity;
        legs.transform.localPosition = legsOffset;
        SetLegHeight();

        var tiltDirection = Mathf.Clamp(rigidbody.velocity.magnitude/2f, 0f, 1f) * ((Mathf.Floor(Time.time * legs.legFps) % 2) * 2f - 1f); // Will give -1 or 1

        tilt = Mathf.Lerp(tilt, tiltDirection, Time.deltaTime * tiltLerpSpeed);

        if (isJumping) tilt = 0f;

        Debug.Log(visualsTransform);
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
        ResetVisualRotation();
    }

    public void ExtendLegs()
    {
        legs.gameObject.SetActive(true);
        legsCollider.enabled = true;
        ResetVisualRotation();
    }

    void ResetVisualRotation()
    {
        if(visualsTransform != null) visualsTransform.localEulerAngles = Vector3.zero;
    }

    void GrowLegs()
    {
        legs = Object.Instantiate(Game.i.library.legsPrefab, parentTransform)
        .GetComponent<Legs>();
        legs.body = parentTransform;
        legs.transform.localPosition = legsOffset;
        foreach (Leg l in legs.legs) l.maxFootDistance = legsHeight + 2f;
    }

    void SetLegHeight()
    {
        legsCollider.size = new Vector3(1f, legsHeight*2f, 1f);
        legsCollider.center = legsOffset + new Vector3(0f, -legsHeight / 2, 0f);
    }
}
