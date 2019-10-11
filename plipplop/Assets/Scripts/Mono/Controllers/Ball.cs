using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : Controller
{
    [Header("Specific properties")]
    public float maxSpeed = 200f;
    public float acceleration = 1000f;
    public float jumpForce = 7f;
    public float airRollFactor = 4f;
    public float moveLerp = 1;

    new Renderer renderer;
    new SphereCollider collider;
    Transform childBall;

    public override void OnPossess()
    {
        renderer.material.color = Color.red;
    }

    public override void OnEject()
    {
        renderer.material.color = Color.white;
    }

    internal override void SpecificMove(Vector3 direction)
    {
        var perimeter = 2 * Mathf.PI * (1f); //radius
        Vector3 _velocity = transform.forward * direction.z * Time.deltaTime + transform.right * direction.x * Time.deltaTime;
        _velocity *= maxSpeed;
        _velocity = Vector3.ClampMagnitude(_velocity, maxSpeed);
        _velocity.y = rigidbody.velocity.y;
        rigidbody.velocity = Vector3.Lerp(rigidbody.velocity, _velocity, Time.deltaTime * moveLerp);
        var factor = 1;
        
        childBall.Rotate(new Vector3(
            (rigidbody.velocity.z / perimeter) * 10f,
            0f,
            - (rigidbody.velocity.x / perimeter) * 10f
        ) * factor, Space.World);
    }

    internal override void OnJump()
    {
        if (IsGrounded()) {
            rigidbody.velocity = new Vector3(rigidbody.velocity.x, jumpForce, rigidbody.velocity.z);
        }
    }

    internal override void Crouch()
    {
        collider.enabled = true;
    }

    internal override void Stand()
    {
        collider.enabled = false;
    }

    bool IsGrounded() {
       return Physics.Raycast(transform.position, Vector3.down, collider.bounds.extents.y + 0.1f);
     }

    internal override void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        renderer = GetComponentInChildren<MeshRenderer>();
        collider = GetComponent<SphereCollider>();
        renderer.material = Instantiate<Material>(renderer.material);
        childBall = transform.GetChild(0);
        base.Start();
    }


    internal override void Update()
    {
        if (IsPossessed())
        {

        }
        base.Update();
    }
}
