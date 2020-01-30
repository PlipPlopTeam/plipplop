using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hopper : Controller
{
    [Header("HOPPER")]
    [Space(5)]
    [Header("Push")]
    public float pushMovementForce = 500f;
    public float pushCooldown = 2f;
    public float pushThreshold = 0.5f;
    public float pushJumpForce = 250f;
    [Space(5)]
    [Header("Others")]
    public float jumpForce = 10f;
    public float speedBeforeLegs = 1f;
    public float rotationVelocityThreshold = 0.25f;
    public float rotationLerpSpeed = 2f;
    public float crouchCooldown = 2f;
    public float drag = 1f;

    float crouchTimer;
    float pushTimer;
    bool pushed;
    bool jumped;

    public override void OnPossess()
    {
        base.OnPossess();
        rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
        transform.eulerAngles = transform.rotation.eulerAngles.y * Vector3.up;
    }

    public override void OnEject()
    {
        base.OnEject();
        rigidbody.constraints = RigidbodyConstraints.None;
    }

    internal override void SpecificMove(Vector3 direction)
    {
        if(!pushed && IsGrounded() && direction.magnitude > pushThreshold)
            Push(direction);
    }

    internal virtual void Push(Vector3 direction)
    {
        Vector3 v = (Game.i.aperture.Right() * direction.x + Game.i.aperture.Forward() * direction.z) * pushMovementForce;
        v += Vector3.up * pushJumpForce;
        rigidbody.AddForce(v * Time.deltaTime, ForceMode.Impulse);
        pushed = true;
        pushTimer = pushCooldown;
    }

    internal override void Update()
    {
        base.Update();

        if(IsPossessed())
        {
            if(pushed)
            {
                pushTimer -= Time.deltaTime;
                if(pushTimer <= 0f) pushed = false;
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
        
        if(!AreLegsRetracted())
        {
			/*
            if(crouchTimer > 0) crouchTimer -= Time.deltaTime;
            else
            {
                if(IsPossessed() && rigidbody.velocity.magnitude <= speedBeforeLegs)
                    RetractLegs();
            }
			*/
        }
        else
        {
            if(IsGrounded(0.5f))
            {
                if(jumped)
                {
                    jumped = false;
                    OnJumpDown();
                }
            }
            else
            {
                if(!jumped) 
                {
                    jumped = true;
                    OnJumpUp();
                }
            }
        }
    }

    internal virtual void OnJumpUp(){}
    internal virtual void OnJumpDown(){}

    internal override void SpecificJump()
    {
        if(IsGrounded())
        {
            ExtendLegs();
            rigidbody.AddForce(Vector3.up * jumpForce * Time.deltaTime, ForceMode.Impulse);
        }
    }

    internal override void OnLegsRetracted()
    {
        rigidbody.drag = drag;
    }

    internal override void OnLegsExtended()
    {
        crouchTimer = crouchCooldown;
    }
}
