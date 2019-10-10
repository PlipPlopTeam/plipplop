using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Glider : Controller
{
    [Header("Specific properties")]
    public float maxFlyingSpeed = 1000f;
    public float thrust = 100f;
    public float baseThrust = 70f;
    public float pitchForce = 10f;
    public float rollForce = 10f;
    public float gravityWhenFlying = 2f;
    public float gravityPlungeFactor = 3f;

    float descentFactor = 0f;
    bool isCrouching = false;
    new Collider collider;

    public override void OnEject()
    {
      //  throw new System.NotImplementedException();
    }

    public override void OnPossess()
    {
       // throw new System.NotImplementedException();
    }

    internal override void Start()
    {
        base.Start();
        collider = GetComponent<Collider>();
        rigidbody = GetComponent<Rigidbody>();
    }

    internal override void Update()
    {
        base.Update();

        descentFactor = (((transform.localEulerAngles.x + 180f) % 360f) / 270f - 0.5f)*2f;

        if (!IsGrounded()) {
            rigidbody.useGravity = false;
            rigidbody.AddForce(Vector3.down * (gravityWhenFlying * (1 - rigidbody.velocity.magnitude / maxFlyingSpeed)) * Time.deltaTime);
            rigidbody.AddForce(transform.forward * baseThrust * (descentFactor + 0.2f) * Time.deltaTime);

            Debug.Log(descentFactor);
            //rigidbody.AddTorque(Vector3.right * (descentFactor) * gravityPlungeFactor * Time.deltaTime);
        }
    }


    internal override void Move(Vector3 direction)
    {
        rigidbody.AddTorque(transform.right * direction.z * pitchForce * Time.deltaTime);
        rigidbody.AddTorque(- transform.forward * direction.x * rollForce * Time.deltaTime);
    }

    internal override void OnHoldJump()
    {
        if (!IsGrounded() && !isCrouching) {
            rigidbody.AddForce(transform.forward * descentFactor * thrust * Time.deltaTime);

            if (rigidbody.velocity.magnitude > maxFlyingSpeed) {
                rigidbody.velocity = rigidbody.velocity.normalized * maxFlyingSpeed;
            }
        }
    }

    internal override void OnJump()
    {
        isCrouching = false;
    }

    internal override void OnToggleCrouch()
    {
        isCrouching = !isCrouching;
        if (isCrouching) {
            Crouch();
        }
        else {
            Stand();
        }
    }

    void Crouch()
    {

    }

    void Stand()
    {

    }

    bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, collider.bounds.extents.y + 0.1f);
    }
}
