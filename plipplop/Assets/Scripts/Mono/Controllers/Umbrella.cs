
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

    Vector3 tiltAccumulation = Vector3.zero;
    Coroutine currentAnimationRoutine = null;

    public override void OnEject()
    {
        base.OnEject();
    }

    public override void OnPossess(bool keepCrouched = false)
    {
        base.OnPossess(keepCrouched);
        ExtendLegs();
    }
    
    internal override void SpecificMove(Vector3 direction)
    {
        Vector3 velocity = (Game.i.aperture.Right() * direction.x + Game.i.aperture.Forward() * direction.z) * airControlSpeed * Time.deltaTime;
        velocity.y = rigidbody.velocity.y;
        rigidbody.velocity = Vector3.Lerp(rigidbody.velocity, velocity, Time.deltaTime * airLerpSpeed);

        tiltAccumulation = Vector3.Lerp(tiltAccumulation, new Vector3(velocity.normalized.z, 0f, -velocity.normalized.x), tiltLerpSpeed * Time.deltaTime);
        visual.eulerAngles = Vector3.zero;
        visual.Rotate(tiltAccumulation * tiltAmplitude, Space.Self);
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
                if (AreLegsRetracted()) ExtendLegs();
            }
            else {
                if (!AreLegsRetracted()) RetractLegs();
            }
        }
    }

    internal override void OnLegsRetracted()
    {
        if (currentAnimationRoutine != null) StopCoroutine(currentAnimationRoutine);
        currentAnimationRoutine = StartCoroutine(OpenUmbrella());
    }

    internal override void OnLegsExtended()
    {
        if (currentAnimationRoutine != null) StopCoroutine(currentAnimationRoutine);
        currentAnimationRoutine = StartCoroutine(CloseUmbrella());
    }

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
}