using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : Controller
{
    [Header("Specific properties")]
    public float maxSpeed = 1000f;
    public float jumpForce = 7f;
    public float airRollFactor = 4f;

    new Rigidbody rigidbody;
    new Renderer renderer;
    SphereCollider collider;
    Transform childBall;

    public override void Move(Vector3 direction)
    {
        rigidbody.AddForce(transform.forward * direction.z * maxSpeed * Time.deltaTime);
        rigidbody.AddForce(transform.right * direction.x * maxSpeed * Time.deltaTime);

        var factor = IsGrounded() ? 1f : airRollFactor;

        childBall.Rotate(new Vector3(
            rigidbody.velocity.z / (Mathf.PI),
            0f,
            - rigidbody.velocity.x / (Mathf.PI)
        ) * factor, Space.World);
    }

    public override void OnPossess()
    {
        renderer.material.color = Color.red;
    }

    public override void OnEject()
    {
        renderer.material.color = Color.white;
    }

    public override void OnJump()
    {
        if (IsGrounded()) {
            rigidbody.AddForce(Vector3.up * jumpForce);
        }
    }

    public override void OnToggleCrouch(bool crouching)
    {
        throw new System.NotImplementedException();
    }


    bool IsGrounded() {
       return Physics.Raycast(transform.position, Vector3.down, collider.bounds.extents.y + 0.1f);
     }

private void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        renderer = GetComponentInChildren<MeshRenderer>();
        collider = GetComponent<SphereCollider>();
        renderer.material = Instantiate<Material>(renderer.material);
        childBall = transform.GetChild(0);
    }

    private void Update()
    {
        if (IsPossessed()) {

        }
    }
}
