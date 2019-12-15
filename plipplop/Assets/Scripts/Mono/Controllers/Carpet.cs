
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Carpet : Controller
{
    public SpringJoint spring;
    public float maxExtension = 0.7f;
    public float muscleSpeed = 1f;
    public float accelerationSpeed = 4f;
    public float cruiseForce = 40f;
    public float turnForce = 10000f;

    float currentZAccumulator = 0f;
    float timeStarted = 0f;

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
        // acc goes from 0 to 1 over time when grounded, and decreases until 0f otherwise
        currentZAccumulator = Mathf.Clamp01(currentZAccumulator + ((direction.z != 0f && IsGrounded() ? (Mathf.Sign(direction.z) * 2f) : 0f) - 1f) * accelerationSpeed * Time.fixedDeltaTime);

        if (IsGrounded()) {
            
            if (currentZAccumulator != 0f) {
                
                // Animation
                if (timeStarted == 0f) {
                    timeStarted = Time.time;
                }

                var muscle = Mathf.Abs(Mathf.Sin((timeStarted-Time.time)*muscleSpeed));
                spring.minDistance = muscle * maxExtension;

                // Actual movement
                var force = transform.forward * cruiseForce * (1f - muscle) * currentZAccumulator * Time.fixedDeltaTime;
                rigidbody.AddForce(force,ForceMode.Acceleration);
            }
            else {
                timeStarted = 0f;
                spring.minDistance = 0f;
            }

            // Rotation
            rigidbody.AddTorque(transform.up * direction.x * (turnForce * (0.3f + currentZAccumulator)) * Time.fixedDeltaTime, ForceMode.Acceleration);
        }
        else {
            spring.minDistance = 0f;
        }
    }

    internal override void OnJump()
    {
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
        if (!IsGrounded() && !AreLegsRetracted()) {
          //RetractLegs();
        } 
    }

    internal override void OnLegsRetracted()
    {
        // Code here
    }

    internal override void OnLegsExtended()
    {
        // Code here
    }
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        var style = new GUIStyle(GUI.skin.box);
        Handles.Label(transform.position + Vector3.up*2f, spring.minDistance.ToString(), style);
    }
#endif
}
