
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
    new public SkinnedMeshRenderer renderer;
    public float flapForce = 200f;

    SkinnedMeshRenderer umbrellaFace;
    Vector3 tiltAccumulation = Vector3.zero;
    Coroutine currentAnimationRoutine = null;
    bool isFlapping = false;
    bool isStuck = false;
    bool isAnimating = false;

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
        visuals.Rotate(new Vector3(computedTilt.z, 0f, -computedTilt.x), Space.Self);
        rigidbody.AddForce(additionalForwardPush * targetDirection);
    }

    internal override void OnJump()
    {
        if (IsGrounded()) {
            if (AreLegsRetracted()) {
                // It's stuck in the ground
                if (isAnimating) StopCoroutine(currentAnimationRoutine);
                currentAnimationRoutine = StartCoroutine(IsDeployed() ? CloseUmbrella() : OpenUmbrella());
            }
            else {
                locomotion.Jump();
            }
        }
        else {
            Flap();
        }
    }

    void Flap()
    {
        if (isFlapping) return;
        if (isAnimating) StopCoroutine(currentAnimationRoutine);
        currentAnimationRoutine = StartCoroutine(FlapRoutine());
    }

    internal override void FixedUpdate()
    {
        // Gravity
        var p = remainingGravityPercentWhenOpened;
        if (rigidbody.velocity.y > 0) {
            p = p + (100 - p) * Mathf.Clamp01(rigidbody.velocity.y);
        }
        ApplyGravity(p / 100f + GetCurrentClosure() * (1f - p / 100f));
        if (-rigidbody.velocity.y > maxFallSpeed * Mathf.Max(p / 100f, GetCurrentClosure())) {
            rigidbody.velocity = Vector3.Scale(Vector3.one - Vector3.up, rigidbody.velocity) + Vector3.down * (maxFallSpeed * Mathf.Max(p / 100f, GetCurrentClosure()));
        }
    }

    internal override void Update()
    {
        base.Update();

        if (IsGrounded()) isFlapping = false;
        if (IsGrounded() && !isStuck) {
            if (AreLegsRetracted()) {
                var surf = locomotion.GetBelowSurface();
                var pos = transform.position;
                if (surf.HasValue) {
                    pos = surf.Value;
                }
                Stuck(pos);
            }
            else {
                if (IsDeployed()) {
                    if (isAnimating) StopCoroutine(currentAnimationRoutine);
                    currentAnimationRoutine = StartCoroutine(CloseUmbrella());
                }
            }
        }
        else if (!IsGrounded()) {
            if (!isFlapping && !IsDeployed() && !isAnimating) {
                currentAnimationRoutine = StartCoroutine(OpenUmbrella());
            }
        }
    }

    internal override void BaseMove(Vector3 direction)
    {
        if (IsGrounded() && !AreLegsRetracted()) {
            locomotion.Move(direction);
        }
        else if (!IsGrounded()){
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
        
        rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
        transform.eulerAngles = transform.rotation.eulerAngles.y * Vector3.up;
    }

    internal override void Start()
    {
        base.Start();
        umbrellaFace = face.GetComponent<SkinnedMeshRenderer>();
    }

    internal override void AlignPropOnHeadDummy()
    {
        if (IsGrounded()) {
            base.AlignPropOnHeadDummy();
        }
    }

    internal override void OnLegsExtended() {
        if (isStuck) {
            UnStuck();
            visuals.localEulerAngles = Vector3.zero;
        }
    }
    internal override void OnLegsRetracted() {
        var surf = locomotion.GetBelowSurface();
        if (surf.HasValue) {
            Stuck(surf.Value);
        }
    }

    void UnStuck()
    {
        transform.Translate(Vector3.up * locomotion.legsHeight);
        isStuck = false;
    }

    void Stuck(Vector3 position)
    {
        transform.position = position;
        visuals.Rotate(new Vector3(Random.value * tiltAmplitude, 0f, -Random.value * tiltAmplitude), Space.Self);
        isStuck = true;
    }

    IEnumerator FlapRoutine()
    {
        isFlapping = true;
        rigidbody.AddForce(Vector3.up * flapForce, ForceMode.Impulse);
        Pyromancer.PlayGameEffect("gfx_umbrella_boost", transform.position + transform.up);
        yield return CloseUmbrella(10f);
        yield return OpenUmbrella();
        isFlapping = false;
    }

    IEnumerator CloseUmbrella(float additionalSpeed=1f)
    {
        isAnimating = true;
        while (renderer.GetBlendShapeWeight(0) < 99f) {
            renderer.SetBlendShapeWeight(0, Mathf.Lerp(renderer.GetBlendShapeWeight(0), 100f, Time.deltaTime*3f* additionalSpeed));
            umbrellaFace.SetBlendShapeWeight(0, Mathf.Lerp(renderer.GetBlendShapeWeight(0), 100f, Time.deltaTime * 3f* additionalSpeed)); 
             yield return null;
        }
        isAnimating = false;
    }

    IEnumerator OpenUmbrella()
    {
        isAnimating = true;
        SoundPlayer.Play("sfx_umbrella_boost");
        while (renderer.GetBlendShapeWeight(0) > 4f) {
            umbrellaFace.SetBlendShapeWeight(0, Mathf.Lerp(renderer.GetBlendShapeWeight(0), 0f, Time.deltaTime * 3f));
            renderer.SetBlendShapeWeight(0, Mathf.Lerp(renderer.GetBlendShapeWeight(0), 0f, Time.deltaTime * 3f));
            yield return null;
        }
        isAnimating = false;
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