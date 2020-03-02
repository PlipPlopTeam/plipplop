using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BaseController : Controller
{
    public float radiusWhenFlat = 0.02f;

    SphereCollider mainCollider;
    PhysicMaterial baseMaterial;
    float originalControllerRadius = 0f;

    public override void OnEject()
    {
        base.OnEject();
        gameObject.SetActive(false);
    }

    public override void OnPossess()
    {
        base.OnPossess();
        gameObject.SetActive(true);
    }

    internal override void SpecificMove(Vector3 direction)
    {
    }

    internal override void Start()
    {
        base.Start();
        // Code here
        mainCollider = GetComponent<SphereCollider>();
        baseMaterial = mainCollider.material;
        mainCollider.radius = originalControllerRadius;
    }

    internal override void Update()
    {
        base.Update();
        // Code here
        AlignPropOnHeadDummy();
        if (AreLegsRetracted()) {
            mainCollider.material = new PhysicMaterial() {
                dynamicFriction = 100f,
                staticFriction = 100f,
                frictionCombine = PhysicMaterialCombine.Maximum,
                bounciness = 0f
            };
            mainCollider.radius = radiusWhenFlat;
            locomotion.isFlattened = true;
        }
        else {
            mainCollider.material = baseMaterial;
            mainCollider.radius = originalControllerRadius;
            locomotion.isFlattened = false;
        }
    }

    internal override void OnLegsRetracted()
    {
        // Code here
    }

    internal override void OnLegsExtended()
    {
        // Code here
    }
}
