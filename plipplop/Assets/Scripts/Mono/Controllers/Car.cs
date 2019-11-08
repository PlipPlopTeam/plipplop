
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Car : Controller
{
    [Header("CAR CONTROLLER properties")]
    [HideInInspector] public float acceleration = 3000f;
    [HideInInspector] public float maxSpeed = 14f;
    [HideInInspector] public float steeringSpeed = 10f;
    [HideInInspector] public float maxSteering = 50f;
    [HideInInspector] public float steeringForce = 1000f;
    [HideInInspector] public float autoBrakeSpeed = 1f;
    [HideInInspector] public float antiSpinSpeed = 6f;
    [HideInInspector] public float tiltAmount = 50f;
    [HideInInspector] public bool antiFlip = true;
    [HideInInspector] public float antiFlipForce = 10f;
    [HideInInspector] public float antiFlipMultiplier = 1f;
    [HideInInspector] public float airStabilizationSpeed = 6f;
    [HideInInspector] public float airStabilizationMultiplier = 0.1f;
    [HideInInspector] public bool canTiltWhenFree = false;
    [HideInInspector] public float highSpeedAdherenceBonusFactor = 1f;

    public Transform[] wheels;

    internal float currentTilt = 0f;
    internal float steering = 0f;

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
        var localSpeed = transform.InverseTransformDirection(rigidbody.velocity);
        var localSpin = transform.InverseTransformDirection(rigidbody.angularVelocity);
        var rotation = transform.eulerAngles;

        // 0<>360  to  -180<>180
        rotation.x = rotation.x > 180f ? rotation.x - 360f : rotation.x;
        rotation.y = rotation.y > 180f ? rotation.y - 360f : rotation.y;
        rotation.z = rotation.z > 180f ? rotation.z - 360f : rotation.z;

        var thrustObjective = localSpeed.z;

        if (!IsGrounded()) {
            direction = Vector3.zero;
        }

        // Thrust
        rigidbody.AddForce(transform.forward * direction.z * acceleration * Time.fixedDeltaTime, ForceMode.Acceleration);

        if (Mathf.Abs(localSpeed.z) > maxSpeed) {
            thrustObjective = Mathf.Sign(localSpeed.z) * maxSpeed;
        }

        // Steering + anti spin
        steering = Mathf.Lerp(steering, direction.x, steeringSpeed * Time.fixedDeltaTime) * (maxSteering/100f);
        localSpin.y = steering * steeringForce * (thrustObjective / maxSpeed) * Time.deltaTime;
        localSpin.y = Mathf.Lerp(localSpin.y, localSpin.y * Mathf.Abs(direction.x), antiSpinSpeed * Time.fixedDeltaTime);

        // Auto brake
        thrustObjective = Mathf.Lerp(thrustObjective, thrustObjective * (Mathf.Abs(direction.z)), autoBrakeSpeed * Time.fixedDeltaTime);
        if (IsGrounded()) localSpeed.x = 0f; // No strafing, good tires

        // Reorient the car to optimal rotation to avoid flipping
        // (Anti flip)
        if (antiFlip && !isImmerged && !IsGrounded() && IsPossessed()) {
            localSpin.x = Mathf.Lerp(localSpin.x, 0f, airStabilizationSpeed * Time.fixedDeltaTime + airStabilizationMultiplier * Mathf.Abs(localSpin.x) * Time.fixedDeltaTime);
            localSpin.z = Mathf.Lerp(localSpin.z, 0f, airStabilizationSpeed * Time.fixedDeltaTime + airStabilizationMultiplier * Mathf.Abs(localSpin.z) * Time.fixedDeltaTime);

            rotation.x = Mathf.Lerp(rotation.x, 0f, antiFlipForce * Time.fixedDeltaTime + antiFlipMultiplier * Mathf.Abs(rotation.x) * Time.fixedDeltaTime);
            rotation.z = Mathf.Lerp(rotation.z, 0f, antiFlipForce * Time.fixedDeltaTime + antiFlipMultiplier * Mathf.Abs(rotation.z) * Time.fixedDeltaTime);
        }

        // Apply
        rigidbody.velocity = transform.TransformDirection(new Vector3(localSpeed.x, localSpeed.y, thrustObjective));
        rigidbody.angularVelocity = transform.TransformDirection(localSpin);
        transform.eulerAngles = rotation;


        // Tilt animation
        TiltVisual(thrustObjective / maxSpeed);
    }

    internal virtual void TiltVisual(float amount)
    {
        currentTilt = Mathf.Lerp(currentTilt, (steering * (maxSteering/100f)) * tiltAmount * (amount), 4f * Time.fixedDeltaTime);
        visuals.localEulerAngles = new Vector3(visuals.localEulerAngles.x, visuals.localEulerAngles.y, currentTilt);
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

    internal override void FixedUpdate()
    {
        base.FixedUpdate();

        // Wheel animation
        var localSpeed = transform.InverseTransformDirection(rigidbody.velocity);
        var factor = 1;
        foreach (var wheel in wheels) {
            wheel.Rotate(new Vector3(0f, 1f, 0f) * localSpeed.z * factor);
        }

        // Even when not controlled, the car shouldn't drift away
        rigidbody.drag = 0f;
        rigidbody.angularDrag = 0f;
        if (!IsGrounded()) rigidbody.angularDrag = 2f;
        if (!IsPossessed()) {
            rigidbody.drag = 4f;
            rigidbody.angularDrag = 2f;

            // No tilting
            if (!canTiltWhenFree) visuals.localEulerAngles = new Vector3(visuals.localEulerAngles.x, visuals.localEulerAngles.y, 0f);
        }

        // Adhere to wall if speed
        locomotion.groundCheckDirection = Vector3.Lerp(Vector3.down, -transform.up, (localSpeed.z / maxSpeed) * highSpeedAdherenceBonusFactor);
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
