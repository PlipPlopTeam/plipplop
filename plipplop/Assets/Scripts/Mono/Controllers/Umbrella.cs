
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Umbrella : Controller
{
    [Header("Specific settings")]
    new public SkinnedMeshRenderer renderer;
    public Transform visual;
    public float airControlSpeed = 5f;
    public float airLerpSpeed = 3f;
    public float tiltAmplitude = 20f;
    public float tiltLerpSpeed = 1f;
    [Range(0f, 100f)] public float remainingGravityPercentWhenClosed = 25f;

    Vector3 tiltAccumulation = Vector3.zero;
    Coroutine currentAnimationRoutine = null;

    public override void OnEject()
    {
        base.OnEject();
    }

    public override void OnPossess()
    {
        base.OnPossess();
        ExtendLegs();
    }
    
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


        visual.localEulerAngles = Vector3.zero;
        var computedTilt = tiltAccumulation * tiltAmplitude;
        if (IsDeployed()) {
            visual.Rotate(new Vector3(computedTilt.z, 0f, -computedTilt.x), Space.Self);
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
        if (IsDeployed()) {
            ApplyGravity(remainingGravityPercentWhenClosed / 100f + GetCurrentClosure() * (1f - remainingGravityPercentWhenClosed / 100f));
        }
        else {
            locomotion.Fall();
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

    internal override void Start()
    {
        base.Start();
    }

    internal override void Update()
    {
        base.Update();

        if (IsPossessed()) {
            if (IsGrounded()) {
                if (currentAnimationRoutine != null) StopCoroutine(currentAnimationRoutine);
                currentAnimationRoutine = StartCoroutine(CloseUmbrella());

                visual.Rotate(new Vector3(-visual.eulerAngles.x, 0f, -visual.eulerAngles.z));
            }
        }
    }

    internal override void OnLegsExtended() {}

    IEnumerator CloseUmbrella()
    {
        while (renderer.GetBlendShapeWeight(0) < 99f) {
            renderer.SetBlendShapeWeight(0, Mathf.Lerp(renderer.GetBlendShapeWeight(0), 100f, Time.deltaTime*3f));
            yield return null;
        }
    }

    IEnumerator OpenUmbrella()
    {
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

    internal override void OnLegsRetracted()
    {
        throw new System.NotImplementedException();
    }
}