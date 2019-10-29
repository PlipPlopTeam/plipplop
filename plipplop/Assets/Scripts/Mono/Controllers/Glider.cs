using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Glider : Controller
{
    [Header("Specific properties")]
    public float maxFlyingSpeed = 1000f;
    public float baseThrust = 70f;
    public float pitchForce = 10f;
    public float rollForce = 10f;
    public float pitchControlInertia = 1f;
    public float rollControlInertia = 1f;
    public float gravityWhenFlying = 2f;
    public float gravityPlungeFactor = 3f;
    public float restabilizationForce = 20f;
    public float turnForce = 5f;
    public float drag = 10f;
    [Range(0,1)] public float pitchMaxAmplitude;

    float descentFactor = 0f;
    Vector3 inertedControls = new Vector3();
    
    public override void OnPossess()
    {
        base.OnPossess();
        // TODO : Fix the glider ground on possess issue
        transform.position += Vector3.up * locomotion.legsHeight;
       // throw new System.NotImplementedException();
    }

    internal override void OnLegsExtended(){}

    internal override void OnLegsRetracted()
    {
        rigidbody.drag = drag;
        //rigidbody.AddForce(transform.forward * 150f * Time.deltaTime, ForceMode.Impulse);
    }
    
    internal override void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        base.Start();
    }

    internal override void Update()
    {
        base.Update();

        if (IsPossessed() && IsGrounded() && AreLegsRetracted())
        {
            ExtendLegs();
        }

        if ((!IsGrounded() || !IsPossessed()) && !AreLegsRetracted())
        {
            RetractLegs();
        }
    }

    internal override void FixedUpdate()
    {
        descentFactor = Mathf.Max(0f, (((transform.localEulerAngles.x + 180f) % 360f) / 270f - 0.55f) - Mathf.Clamp01(rigidbody.velocity.y)) *4f; // Magic

        // To avoid the glider running away on spawn
        float propulsionFactor = IsPossessed() ? 1f : 0.2f;

        if (AreLegsRetracted() && rigidbody.velocity.magnitude != 0f) {

            // Fake gravity
            // Must be on
            if (useGravity) rigidbody.AddForce(Vector3.down * (gravityWhenFlying/rigidbody.velocity.magnitude) * Time.deltaTime);
            
            // Base thrust
            rigidbody.AddForce(transform.forward * propulsionFactor * baseThrust * (descentFactor + 0.1f) * Time.deltaTime);

            // Plunge
            var targetDescentFactor = 3f - rigidbody.velocity.magnitude / 2f;
            rigidbody.AddTorque(transform.right * (0.85f - Mathf.Abs(inertedControls.z)) * (targetDescentFactor - descentFactor) * propulsionFactor * gravityPlungeFactor * Time.deltaTime);

            // Natural restabilization
            var angle = (transform.localEulerAngles.z - 180f)% 360f;
            rigidbody.AddTorque(transform.forward * (1f - Mathf.Abs(inertedControls.x)) * propulsionFactor * Mathf.Clamp(angle / 50f, -1f, 1f) * restabilizationForce * Time.deltaTime);
        }
        

        if (rigidbody.velocity.magnitude > maxFlyingSpeed * propulsionFactor) {
            rigidbody.velocity = rigidbody.velocity.normalized * maxFlyingSpeed;
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (EditorApplication.isPlaying) {

            try {
                var normalizedRoll = (((transform.localEulerAngles.z - 180f) % 360f) / 180f);
                normalizedRoll -= Mathf.Sign(normalizedRoll) * 0.75f;
                normalizedRoll *= 2f;

                Handles.Label(transform.position + Vector3.up, string.Join("\n", new string[] {
                    string.Format("Magnitude {0}", rigidbody.velocity.magnitude),
                    string.Format("Descent factor {0}", descentFactor),
                    string.Format("Target factor {0}", 1f - rigidbody.velocity.magnitude / 5f),
                    string.Format("Inerted {0}", inertedControls),
                    string.Format("Roll {0}", - transform.forward * inertedControls.x * Mathf.Abs(normalizedRoll) * rollForce * Time.deltaTime),
                })
                );
            }
            catch { }
        }
    }
#endif


    internal override void SpecificMove(Vector3 direction)
    {
        inertedControls.z = Mathf.Lerp(inertedControls.z, direction.z, pitchControlInertia * Time.deltaTime);
        inertedControls.x = Mathf.Lerp(inertedControls.x, direction.x, rollControlInertia * Time.deltaTime);
        inertedControls.z = Mathf.Clamp(inertedControls.z, -pitchMaxAmplitude, pitchMaxAmplitude);

        rigidbody.AddTorque(transform.right * inertedControls.z * pitchForce * Time.deltaTime);

        // Can only go sideways as far as to not completely roll over the glider
        var normalizedRoll = (((transform.localEulerAngles.z - 180f) % 360f) / 180f);
        normalizedRoll -= Mathf.Sign(normalizedRoll) * 0.75f;
        normalizedRoll *= 2f;
        rigidbody.AddTorque(- transform.forward * inertedControls.x * Mathf.Abs(normalizedRoll) * rollForce * Time.deltaTime);

        rigidbody.AddTorque(Vector3.up * inertedControls.x * (Mathf.Abs(normalizedRoll) + 0.1f) * rollForce * turnForce * Time.deltaTime);


    }
}
