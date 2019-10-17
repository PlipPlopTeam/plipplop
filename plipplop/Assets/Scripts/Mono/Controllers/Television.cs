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
    public float drag = 1f;
    [Space(5)]
    [Header("Referencies")]
    public Animation anim;
    public ParticleSystem ps;
    public Valuable valuable;
    public Renderer screenRenderer;

    [Space(5)]
    [Header("Screen")]
    public Texture standScreenTexture;
    public Texture crouchScreenTexture;
    public Texture idleScreenTexture;

    float crouchTimer;
    float pushTimer;
    bool pushed;
    bool jumped;

    public override void OnEject()
    {
        base.OnEject();
        screenRenderer.material.mainTexture = idleScreenTexture;
        // Code here
    }

    public override void OnPossess()
    {
        base.OnPossess();
        // Code here
    }

    internal override void SpecificMove(Vector3 direction)
    {
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
        screenRenderer.material = Instantiate(screenRenderer.material);
        // Code here
    }

    void OnDestroy() 
    {
        Destroy(screenRenderer.material);
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
            crouchTimer -= Time.deltaTime;
            if(crouchTimer <= 0)
            {
                if(IsPossessed() && rigidbody.velocity.magnitude <= speedBeforeLegs)
                    RetractLegs();
            }
        }
        else
        {
            if(IsGrounded(0.5f))
            {
                if(jumped)
                {
                    jumped = false;
                    ps.Play();
                    valuable.hidden = true;
                }
            }
            else
            {
                if(!jumped) 
                {
                    valuable.hidden = false;
                    jumped = true;
                }
                
            }
        }
    }

    internal override void SpecificJump()
    {
        if(AreLegsRetracted())
        {
            ExtendLegs();
            rigidbody.AddForce(Vector3.up * jumpForce * Time.deltaTime, ForceMode.Impulse);
        }
    }

    internal override void OnLegsRetracted()
    {
        // Code here
        valuable.hidden = true;
        rigidbody.drag = drag;
        screenRenderer.material.mainTexture = crouchScreenTexture;
    }

    internal override void OnLegsExtended()
    {
        // Code here
        crouchTimer = crouchCooldown;
        valuable.hidden = false;
        screenRenderer.material.mainTexture = standScreenTexture;
    }
}
