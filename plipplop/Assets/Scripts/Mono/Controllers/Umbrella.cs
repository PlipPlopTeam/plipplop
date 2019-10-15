
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Umbrella : Controller
{
    [Header("Specific settings")]
    new public SkinnedMeshRenderer renderer;

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

    }

    internal override void SpecificJump()
    {

    }

    internal override void Start()
    {
        base.Start();
    }

    internal override void Update()
    {
        base.Update();
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
        while (renderer.GetBlendShapeWeight(0) < 1f) {
            renderer.SetBlendShapeWeight(0, Mathf.Lerp(renderer.GetBlendShapeWeight(0), 0f, Time.deltaTime * 3f));
            yield return null;
        }
    }
}