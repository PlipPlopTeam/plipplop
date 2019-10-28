using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Longboard : Car
{
    [Header("Sub-specific properties")]
    public float jumpForce = 15f;
    public float airRollAccumulatorSpeed = 2f;
    public float airRollForce = 600f;
    public float airPitchForce = 600f;


    internal override void SpecificJump()
    {
        base.SpecificJump();
        if (IsGrounded()) {
            rigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Acceleration);
        }
    }

    internal override void SpecificMove(Vector3 direction)
    {
        if (!IsGrounded()) {
            AirPitchAndRoll(direction);
        }
        base.SpecificMove(direction);
    }

    void AirPitchAndRoll(Vector3 direction)
    {
        // Pitch and roll
        rigidbody.AddTorque(transform.right * direction.z * Time.fixedDeltaTime * airPitchForce, ForceMode.Acceleration);
        rigidbody.AddTorque(-transform.forward * direction.x * Time.fixedDeltaTime * airRollForce, ForceMode.Acceleration);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * 5f);
    }
}
