using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Longboard : Car
{
    [Header("Sub-specific properties")]
    public float jumpForce = 15f;
    public float airRollAccumulatorSpeed = 2f;

    float airRollAccumulator = 0f;

    internal override void SpecificJump()
    {
        base.SpecificJump();
        if (IsGrounded()) {
            rigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Acceleration);
        }
    }

    internal override void AirPitchAndRoll(Vector3 direction)
    {
        // Pitch and roll
        rigidbody.AddTorque(transform.right * direction.z * Time.fixedDeltaTime * airPitchForce, ForceMode.Acceleration);
        rigidbody.AddTorque(-transform.forward * direction.x * Time.fixedDeltaTime * airRollForce, ForceMode.Acceleration);
    }
}
