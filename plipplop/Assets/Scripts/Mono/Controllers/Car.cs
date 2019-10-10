using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car : Controller
{
    [Header("Specific properties")]
    public float maxSpeed = 1000f;
    public float acceleration = 100f;
    public float jumpForce = 100f;
    
    float speed;

    internal override void Start()
    {
        base.Start();

        rigidbody = GetComponent<Rigidbody>();
    }

    internal override void Move(Vector3 direction)
    {
        if (direction.magnitude > 0f) {
            speed += Time.deltaTime * acceleration;
            if (speed > maxSpeed) speed = maxSpeed;
        }
        rigidbody.AddForce(direction * speed * Time.deltaTime);
    }

    internal override void OnJump()
    {
        rigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }


    public override void OnEject()
    {
        //Nothing for now
    }

    public override void OnPossess()
    {
        //Nothing for now
    }
}
