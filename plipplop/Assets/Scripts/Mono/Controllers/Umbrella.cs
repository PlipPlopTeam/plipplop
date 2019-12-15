
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Umbrella : Controller
{
    [Header("Specific settings")]
    public float airControlSpeed = 5f;
    public float airLerpSpeed = 3f;
    public float tiltAmplitude = 20f;
    public float tiltLerpSpeed = 1f;
    public float maxFallSpeed = 20f;
    [Range(0f, 100f)] public float remainingGravityPercentWhenOpened = 25f;
    public float additionalForwardPush = 10f;


    new SkinnedMeshRenderer renderer;
    Vector3 tiltAccumulation = Vector3.zero;
    Coroutine currentAnimationRoutine = null;

    void AirMove(Vector3 direction)
    {
        tiltAccumulation = Vector3.Lerp(tiltAccumulation, new Vector3(direction.x, 0f, direction.z), tiltLerpSpeed * Time.fixedDeltaTime);

        Vector3 velocity = (Game.i.aperture.Right() * tiltAccumulation.x + Game.i.aperture.Forward() * tiltAccumulation.z) * airControlSpeed * Time.fixedDeltaTime;

        velocity.y = rigidbody.velocity.y;
        rigidbody.velocity = Vector3.Lerp(rigidbody.velocity, velocity, Time.fixedDeltaTime * airLerpSpeed);


        Vector3 clampDirection = Vector3.ClampMagnitude(tiltAccumulation, 1f);
        Vector3 dir = tiltAccumulation.x * Game.i.aperture.Right() + tiltAccumulation.z * Game.i.aperture.Forward();
        
        // Rotate legs
        var targetDirection = dir;

        transform.forward = Vector3.Lerp(transform.forward, targetDirection, Time.deltaTime * 4f);

        visuals.localEulerAngles = Vector3.zero;
        var computedTilt = tiltAccumulation * tiltAmplitude;
        if (IsDeployed()) {
            visuals.Rotate(new Vector3(computedTilt.z, 0f, -computedTilt.x), Space.Self);
            rigidbody.AddForce(additionalForwardPush * targetDirection);
        }
    }

    internal override void OnJump()
    {
        if (!IsGrounded()) {
            if (!IsDeployed()) {
                if (currentAnimationRoutine != null) StopCoroutine(currentAnimationRoutine);
                currentAnimationRoutine = StartCoroutine(OpenUmbrella());
            }
            else {
                if (currentAnimationRoutine != null) StopCoroutine(currentAnimationRoutine);
                currentAnimationRoutine = StartCoroutine(CloseUmbrella());
            }
        }
        else {
            locomotion.Jump();
        }
    }

    internal override void FixedUpdate()
    {
        ApplyGravity(remainingGravityPercentWhenOpened / 100f + GetCurrentClosure() * (1f - remainingGravityPercentWhenOpened / 100f));
        if (-rigidbody.velocity.y > maxFallSpeed * Mathf.Max(remainingGravityPercentWhenOpened/100f, GetCurrentClosure())) {
            rigidbody.velocity = Vector3.Scale(Vector3.one - Vector3.up, rigidbody.velocity) + Vector3.down * (maxFallSpeed * Mathf.Max(remainingGravityPercentWhenOpened / 100f, GetCurrentClosure()));
        }
    }

    internal override void BaseMove(Vector3 direction)
    {
        if (IsGrounded()) {
            locomotion.Move(direction);
        }
        else {
            AirMove(direction);
        }

    }

    public override void OnEject()
    {
        base.OnEject();
        rigidbody.constraints = RigidbodyConstraints.None;
    }

    public override void OnPossess()
    {
        base.OnPossess();

        // TODO: Remove this shit
        transform.position += Vector3.up * locomotion.legsHeight;
        // 

        rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
        transform.eulerAngles = transform.rotation.eulerAngles.y * Vector3.up;
    }

    internal override void Start()
    {
        base.Start();
        renderer = visuals.GetComponentInChildren<SkinnedMeshRenderer>();
    }

    internal override void Update()
    {
        base.Update();

        if (IsPossessed()) {
            if (IsGrounded()) {
                if (AreLegsRetracted()) ExtendLegs();
                if (currentAnimationRoutine != null) StopCoroutine(currentAnimationRoutine);
                currentAnimationRoutine = StartCoroutine(CloseUmbrella());
            }
        }
        else {
            if (!AreLegsRetracted()) {
                RetractLegs();
            }
            /*
            if (IsDeployed() && (Mathf.Abs(transform.rotation.eulerAngles.x) + Mathf.Abs(transform.rotation.eulerAngles.z)) > 30f) {
                if (currentAnimationRoutine != null) StopCoroutine(currentAnimationRoutine);
                currentAnimationRoutine = StartCoroutine(CloseUmbrella());
            }
            */
        }
    }

    internal override void AlignPropOnHeadDummy()
    {
        if (IsGrounded()) {
            base.AlignPropOnHeadDummy();
        }
    }

    internal override void OnLegsExtended() { }
    internal override void OnLegsRetracted() { }

    IEnumerator CloseUmbrella()
    {
        while (renderer.GetBlendShapeWeight(0) < 99f) {
            renderer.SetBlendShapeWeight(0, Mathf.Lerp(renderer.GetBlendShapeWeight(0), 100f, Time.deltaTime*3f));
            yield return null;
        }
    }

    IEnumerator OpenUmbrella()
    {
        SoundPlayer.Play("sfx_umbrella_boost");
        while (renderer.GetBlendShapeWeight(0) > 1f) {
            renderer.SetBlendShapeWeight(0, Mathf.Lerp(renderer.GetBlendShapeWeight(0), 0f, Time.deltaTime * 3f));
            yield return null;
        }
    }

    float GetCurrentClosure()
    {
        return renderer.GetBlendShapeWeight(0) / 100f;
    }

    bool IsDeployed()
    {
        return GetCurrentClosure() < 0.5F;
    }

}