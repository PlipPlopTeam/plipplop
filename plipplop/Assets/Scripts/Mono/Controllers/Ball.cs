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

    new Rigidbody rigidbody;
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

    internal override void Move(Vector3 direction)
    {
        var perimeter = 2 * Mathf.PI * (1f); //radius
        var control = IsGrounded() ? 1f : 0.25f;

        rigidbody.AddForce(control * transform.forward * direction.z * acceleration * Time.deltaTime);
        rigidbody.AddForce(control * transform.right * direction.x * acceleration * Time.deltaTime);

        if (rigidbody.velocity.magnitude > maxSpeed) {
            rigidbody.velocity = rigidbody.velocity.normalized * maxSpeed;
        }

        var factor = IsGrounded() ? 1f : airRollFactor;

        childBall.Rotate(new Vector3(
            (rigidbody.velocity.z / perimeter) * 10f,
            0f,
            - (rigidbody.velocity.x / perimeter) * 10f
        ) * factor, Space.World);
    }

    internal override void OnJump()
    {
        if (IsGrounded()) {
            rigidbody.AddForce(Vector3.up * jumpForce);
        }
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
        if (IsPossessed()) {

        }

        base.Update();
    }
}
