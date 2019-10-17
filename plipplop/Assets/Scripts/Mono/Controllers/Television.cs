using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Television : Controller
{
    [Header("Specific properties")]

    [Header("Push")]
    public float pushMovementForce = 10f;
    public float pushCooldown = 2f;
    public float pushThreshold = 0.5f;
    public float pushJumpForce = 10f;
    [Space(5)]
    [Header("Others")]
    public float jumpForce = 10f;
    public float speedBeforeLegs;
    public float rotationVelocityThreshold = 0.25f;
    public float rotationLerpSpeed = 2f;
    public float crouchCooldown = 2f;
    [Space(5)]
    [Header("Referencies")]
    public Animation anim;
    public ParticleSystem ps;
    public Valuable valuable;

    float crouchTimer;
    float pushTimer;
    bool pushed;
    bool jumped;

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
        //Vector3 v = (Game.i.aperture.Right() * direction.x + Game.i.aperture.Forward() * direction.z);
        //rigidbody.AddForce(v * speed * Time.deltaTime);
        if(!pushed && direction.magnitude > pushThreshold)
        {
            Vector3 v = (Game.i.aperture.Right() * direction.x + Game.i.aperture.Forward() * direction.z) * pushMovementForce;
            v += Vector3.up * pushJumpForce;
            rigidbody.AddForce(v * Time.deltaTime, ForceMode.Impulse);
            anim.Play();
            pushed = true;
            pushTimer = pushCooldown;
        }

    }

    internal override void Start()
    {
        base.Start();
        // Code here
    }

    internal override void Update()
    {
        base.Update();

        if(!AreLegsRetracted())
        {
            crouchTimer -= Time.deltaTime;
            if(crouchTimer <= 0)
            {
                if(IsPossessed() && rigidbody.velocity.magnitude <= speedBeforeLegs)
                    RetractLegs();
            }
        }

        if(pushed)
        {
            pushTimer -= Time.deltaTime;
            if(pushTimer <= 0f) pushed = false;
        }

        if(IsGrounded())
        {
            if(jumped)
            {
                jumped = false;
                ps.Play();
            }
        }
        else
        {
            if(!jumped) jumped = true;
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

    internal override void OnJump()
    {
        if(AreLegsRetracted())
        {
            ExtendLegs();
            rigidbody.AddForce(Vector3.up * jumpForce * Time.deltaTime, ForceMode.Impulse);
        }
        else
        {
            rigidbody.AddForce(Vector3.up * jumpForce * 2f * Time.deltaTime, ForceMode.Impulse);
        }

    }

    internal override void OnLegsRetracted()
    {
        // Code here
        valuable.hidden = true;
    }

    internal override void OnLegsExtended()
    {
        // Code here
        crouchTimer = crouchCooldown;
        valuable.hidden = false;
    }
}
