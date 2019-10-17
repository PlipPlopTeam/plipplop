using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Television : Controller
{
    [Header("Specific properties")]
    public float speed = 10f;
    public float speedBeforeLegs;
    public float rotationVelocityThreshold = 0.25f;
    public float rotationLerpSpeed = 2f;

    public override void OnEject()
    {
        base.OnEject();
        // Code here
    }

    public override void OnPossess()
    {
        base.OnPossess();
        // Code here
    }

    internal override void SpecificMove(Vector3 direction)
    {
        Vector3 v = (Game.i.aperture.Right() * direction.x + Game.i.aperture.Forward() * direction.z);
        rigidbody.AddForce(v * speed * Time.deltaTime);
    }

    internal override void Start()
    {
        base.Start();
        // Code here
    }

    internal override void Update()
    {
        base.Update();

        if(IsPossessed())
        {
            if(rigidbody.velocity.magnitude > speedBeforeLegs)
            {
                if(AreLegsRetracted()) ExtendLegs();
            }
            else 
            {
                if(!AreLegsRetracted()) RetractLegs();
            }
        }

        if(rigidbody.velocity.magnitude > rotationVelocityThreshold)
        {
            Vector3 moveDirection = rigidbody.velocity.normalized;
            transform.forward = Vector3.Lerp(
                transform.forward,
                new Vector3(moveDirection.x, 0f, moveDirection.z),
                Time.deltaTime * rotationLerpSpeed);
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
