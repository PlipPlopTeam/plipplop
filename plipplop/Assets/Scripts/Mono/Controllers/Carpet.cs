
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
    public float decelerationSpeed = 50f;
    public float centralBoneForce = 10000f;

    public Rigidbody[] rigidbodies;
    public Rigidbody centralBone;
    public PhysicMaterial playMaterial;
    public PhysicMaterial immobileMaterial;

    float currentZAccumulator = 0f;
    float timeStarted = 0f;

    public Transform head;


    public override void OnEject()
    {
        base.OnEject();
        foreach(var collider in GetComponentsInChildren<Collider>()) {
            collider.material = immobileMaterial;
        }
        
        // Code here
    }

    public override void OnPossess()
    {
        base.OnPossess();
        foreach (var collider in GetComponentsInChildren<Collider>()) {
            if (collider.gameObject == gameObject) continue;
            collider.material = playMaterial;
        }
        // Code here
    }

    internal override void BaseMove(Vector3 direction)
    {
        base.BaseMove(direction);
    }

    internal override void SpecificMove(Vector3 direction)
    {
        // acc goes from 0 to 1 over time when grounded, and decreases until 0f otherwise
        currentZAccumulator = Mathf.Clamp(currentZAccumulator + ((direction.z != 0f && IsGrounded() ? (Mathf.Sign(direction.z) * 2f - 1f) : 0f)) * accelerationSpeed * Time.fixedDeltaTime, -1f, 1f);
        if (direction.z == 0F) {
            currentZAccumulator *= Mathf.Min(1f/Time.deltaTime, 1f/decelerationSpeed) * Time.deltaTime;
        }

        if (IsGrounded()) {
            if (currentZAccumulator != 0f) {
                
                // Animation
                if (timeStarted == 0f) {
                    timeStarted = Time.time;
                }

                var muscle = Mathf.Abs(Mathf.Sin((timeStarted-Time.time)*muscleSpeed));
                spring.minDistance = muscle * maxExtension;

                centralBone.AddForce(Vector3.up * centralBoneForce * muscle);

                // Actual movement
                var y = transform.eulerAngles.y;
                var norm = locomotion.GetGroundNormal();
                if (norm.HasValue) {
                    transform.up = norm.Value;
                    transform.Rotate(Vector3.up * y);
                }

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
            //spring.minDistance = 0f;
        }
    }

    internal override void Start()
    {
        base.Start();
        foreach (var collider in GetComponentsInChildren<Collider>()) {
            if (collider.gameObject == gameObject) continue;
            collider.material = immobileMaterial;
        }
        // Code here
    }

    internal override void Update()
    {
        base.Update();
    }

    internal override void OnLegsRetracted()
    {
        head.localPosition = new Vector3(0f, 0f, 0f);
    }

    internal override void OnLegsExtended()
    {
        // Code here
        transform.position += Vector3.up * 0.3f;
        foreach (var rb in rigidbodies) {
            rb.isKinematic = false;
        }

        head.localPosition = visualsOffset;
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        var style = new GUIStyle(GUI.skin.box);
        Handles.Label(transform.position + Vector3.up*2f, spring.minDistance.ToString(), style);
    }
#endif
}
