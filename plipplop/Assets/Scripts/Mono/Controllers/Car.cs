using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car : Controller
{
    [Header("Specific properties")]
    public float maxSpeed = 1000f;
    public float acceleration = 100f;
    public float jumpForce = 100f;
    
    new Rigidbody rigidbody;
    float speed;

    private void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    public override void Move(Vector3 direction)
    {
        if (direction.magnitude > 0f) {
            speed += Time.deltaTime * acceleration;
            if (speed > maxSpeed) speed = maxSpeed;
        }
        rigidbody.AddForce(direction * speed * Time.deltaTime);
    }

    public override void OnJump()
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

    public override void OnToggleCrouch(bool crouching)
    {
        throw new System.NotImplementedException();
    }
}
