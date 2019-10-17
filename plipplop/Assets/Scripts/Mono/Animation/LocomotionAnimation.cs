using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocomotionAnimation
{
    public float legsHeight;
    public Vector3 legsOffset;
    public bool isJumping;

    Transform parentTransform;
    Rigidbody rigidbody;
    CapsuleCollider legsCollider;
    Legs legs;

    public LocomotionAnimation(CapsuleCollider legsCollider)
    {
        this.legsCollider = legsCollider;
        parentTransform = legsCollider.transform;
        rigidbody = legsCollider.GetComponent<Rigidbody>();
        legsCollider.material = new PhysicMaterial() { dynamicFriction = 0f, staticFriction = 0f, frictionCombine = PhysicMaterialCombine.Minimum };
        GrowLegs();
    }

    public void Update()
    {
        legs.velocity = rigidbody.velocity;
        SetLegHeight();
    }

    public bool AreLegsRetracted()
    {
        return legs == null || !legs.gameObject.activeSelf;
    }

    public void RetractLegs()
    {
        legs.gameObject.SetActive(false);
        legsCollider.enabled = false;
    }

    public void ExtendLegs()
    {
        legs.gameObject.SetActive(true);
        legsCollider.enabled = true;
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
        legsCollider.height = legsHeight;
        legsCollider.center = legsOffset + new Vector3(0f, -legsHeight / 2, 0f);
    }
}
