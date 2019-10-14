﻿using UnityEngine;

public class Ball : Controller
{
    [Header("Specific properties")]
    public float maxSpeed = 200f;
    public float acceleration = 1000f;
    public float jumpForce = 7f;
    public float airRollFactor = 4f;
    public float moveLerp = 1;
    public float drag = 1f;
    public float speedBeforeRoll = 12f;
    public float speedBeforeStandingUp = 2f;

    new Renderer renderer;
    new SphereCollider collider;
    Transform childBall;
    float originalLegHeight = 2f;

    public override void OnPossess(bool wasCrouching=false)
    {
        base.OnPossess(wasCrouching);
        ExtendLegs();
    }

    public override void OnEject()
    {
        base.OnEject();
    }

    internal override void SpecificMove(Vector3 direction)
    {
        Vector3 clampDirection = Vector3.ClampMagnitude(direction, 1f);
        
        var perimeter = 2 * Mathf.PI * (1f); //radius
        Vector3 _velocity = (Game.i.aperture.Right() * direction.x + Game.i.aperture.Forward() * direction.z) * Time.deltaTime;
        _velocity *= maxSpeed;
        _velocity = Vector3.ClampMagnitude(_velocity, maxSpeed);
        _velocity.y = rigidbody.velocity.y;
        rigidbody.velocity = Vector3.Lerp(rigidbody.velocity, _velocity, Time.deltaTime * moveLerp);

        if (rigidbody.velocity.magnitude > 0f) transform.forward = rigidbody.velocity.normalized;

        var factor = 1;
        
        childBall.Rotate(new Vector3(
            (rigidbody.velocity.z / perimeter) * 10f,
            0f,
            - (rigidbody.velocity.x / perimeter) * 10f
        ) * factor, Space.World);
    }

    internal override void SpecificJump()
    {
        if(IsGrounded()) 
        {
            rigidbody.velocity = new Vector3(rigidbody.velocity.x, jumpForce, rigidbody.velocity.z);
        }
    }

    internal override void OnLegsRetracted()
    {
        collider.enabled = true;
        rigidbody.drag = drag;
    }

    internal override void OnLegsExtended()
    {
        collider.enabled = false;
        rigidbody.drag = locomotion.baseDrag;
    }

    internal override void Awake()
    {
        base.Awake();

        rigidbody = GetComponent<Rigidbody>();
        renderer = GetComponentInChildren<MeshRenderer>();
        collider = GetComponent<SphereCollider>();
        renderer.material = Instantiate<Material>(renderer.material);
        childBall = transform.GetChild(0);
    }

    internal override void Start()
    {
        base.Start();
        originalLegHeight = legsHeight;
    }

    internal override void FixedUpdate() {}

    internal override void Update()
    {
        base.Update();

        if (IsPossessed())
        {
            legsHeight = originalLegHeight * (1f - rigidbody.velocity.magnitude / speedBeforeRoll);
            if (rigidbody.velocity.magnitude > speedBeforeRoll) {
                RetractLegs();
            }
            else if (IsGrounded() && rigidbody.velocity.magnitude < speedBeforeStandingUp) {
                ExtendLegs();
            }
        }
    }
}
