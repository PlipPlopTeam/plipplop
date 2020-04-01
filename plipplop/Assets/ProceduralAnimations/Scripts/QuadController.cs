using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuadController : MonoBehaviour
{
    public float speed;

    public Rigidbody rb;

    public float rotationSpeed;

    private Vector3 movement;

    private Vector2 inputs;

    public float lerpSpeed;

    public bool walking;
    public bool turning;

    public float distanceFromGround;
    public float pushForce = 1;

    public Transform velocityTransform;


    public float frameRate;
    
    
    private void Update()
    {
        inputs = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

    }

    private void FixedUpdate()
    {
        Levitate();
        TiltTowardVelocity();
        
        if (inputs.y != 0)
        {
            Move();
        }
       

       // movement.y = rb.velocity.y;

        //rb.velocity = Vector3.Lerp(rb.velocity, movement, lerpSpeed);    


        if (inputs.x != 0)
        {
            Turn();
            if (!turning) turning = true;
        }
        else
        {
            if (turning) turning = false;
        }
    }

    void Move()
    {
       // movement = transform.forward * inputs.y * speed;
        rb.AddForce(transform.forward * inputs.y * speed, ForceMode.Acceleration);
        if (!walking)
        {
            walking = true;
        }
    }

    void Stop()
    {
        movement = Vector3.zero;
        if (walking)
        {
            walking = false;
        }
        
    }
    
    void Turn()
    {
        transform.Rotate(Vector3.up * rotationSpeed * inputs.x);
    }

    void Levitate()
    {
        RaycastHit hit;
        
        if (Physics.Raycast(transform.position, Vector3.down, out hit, distanceFromGround))
        {
            float _factor = 1/hit.distance/distanceFromGround;
            rb.AddForce(Vector3.up * pushForce * _factor);
        }
    }

    public float maxTiltValue = 10;

    void TiltTowardVelocity()
    {
        
        velocityTransform.localEulerAngles = new Vector3( 
            Vector3.Dot(rb.velocity, transform.forward)* maxTiltValue,
            velocityTransform.localEulerAngles.y,
            Vector3.Dot(rb.velocity, transform.right) * maxTiltValue );
    }
}
