
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CounterStrike : Controller
{
    public float speed = 100f;
    public float jumpForce = 500f;
    public float cameraSensi = 2f;

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
        rigidbody.velocity = transform.forward * direction.z * speed  + transform.right * direction.x * speed + Vector3.up * rigidbody.velocity.y;
    }

    internal override void SpecificJump()
    {
        if (!IsGrounded()) return;
        rigidbody.velocity = new Vector3(rigidbody.velocity.x, jumpForce, rigidbody.velocity.z);
    }

    internal override void Start()
    {
        base.Start();
        // Code here
    }

    internal override void Update()
    {
        base.Update();
        // Code here
    }

    internal override void MoveCamera(Vector2 d)
    {
        rigidbody.AddTorque(Vector3.up * cameraSensi * d.x);
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
