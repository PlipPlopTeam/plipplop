using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Glider : Controller
{
    [Header("Specific properties")]
    public float maxFlyingSpeed = 1000f;
    public float thrust = 100f;
    public float baseThrust = 70f;
    public float pitchForce = 10f;
    public float rollForce = 10f;
    public float pitchControlInertia = 1f;
    public float rollControlInertia = 1f;
    public float gravityWhenFlying = 2f;
    public float gravityPlungeFactor = 3f;
    public float restabilizationForce = 20f;
    public float turnForce = 5f;

    float descentFactor = 0f;
    Vector3 inertedControls = new Vector3();
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

        descentFactor = Mathf.Max(0f, (((transform.localEulerAngles.x + 180f) % 360f) / 270f - 0.55f))*4f; // Magic

        if (!IsGrounded()) {

            // Fake gravity
            rigidbody.useGravity = false;
            rigidbody.AddForce(Vector3.down * (gravityWhenFlying/rigidbody.velocity.magnitude) * Time.deltaTime);
            
            // Base thrust
            rigidbody.AddForce(transform.forward * baseThrust * (descentFactor + 0.1f) * Time.deltaTime);

            // Plunge
            var targetDescentFactor = 3f - rigidbody.velocity.magnitude / 2f;
            rigidbody.AddTorque(transform.right * (0.85f - Mathf.Abs(inertedControls.z)) * (targetDescentFactor - descentFactor) * gravityPlungeFactor * Time.deltaTime);

            // Natural restabilization
            var angle = (transform.localEulerAngles.z - 180f)% 360f;
            rigidbody.AddTorque(transform.forward * (1f - Mathf.Abs(inertedControls.x)) * Mathf.Clamp(angle / 50f, -1f, 1f) * restabilizationForce * Time.deltaTime);
        }
        else {
            rigidbody.useGravity = true;
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (EditorApplication.isPlaying) {
            var normalizedRoll = (((transform.localEulerAngles.z - 180f) % 360f) / 180f);
            normalizedRoll -= Mathf.Sign(normalizedRoll) * 0.5f;
            normalizedRoll *= 2f;
            normalizedRoll = Mathf.Sign(normalizedRoll) - normalizedRoll;

            Handles.Label(transform.position + Vector3.up, string.Join("\n", new string[] {
                    string.Format("Gravity {0}", gravityWhenFlying/rigidbody.velocity.magnitude),
                    string.Format("Magnitude {0}", rigidbody.velocity.magnitude),
                    string.Format("Descent factor {0}", descentFactor),
                    string.Format("Target factor {0}", 1f - rigidbody.velocity.magnitude / 5f),
                    string.Format("Inerted {0}", inertedControls),
                    string.Format("Roll {0}", normalizedRoll),
                })
            );
        }
    }


    internal override void Move(Vector3 direction)
    {
        inertedControls.z = Mathf.Lerp(inertedControls.z, direction.z, pitchControlInertia * Time.deltaTime);
        inertedControls.x = Mathf.Lerp(inertedControls.x, direction.x, rollControlInertia * Time.deltaTime);

        rigidbody.AddTorque(transform.right * inertedControls.z * pitchForce * Time.deltaTime);

        // Can only go sideways as far as to not completely roll over the glider
        var normalizedRoll = (((transform.localEulerAngles.z - 180f) % 360f) / 180f);
        normalizedRoll -= Mathf.Sign(normalizedRoll) * 0.75f;
        normalizedRoll *= 2f;
        rigidbody.AddTorque(- transform.forward * inertedControls.x * Mathf.Abs(normalizedRoll) * rollForce * Time.deltaTime);

        rigidbody.AddTorque(Vector3.up * inertedControls.x * Mathf.Abs(normalizedRoll) * rollForce * turnForce * Time.deltaTime);
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
