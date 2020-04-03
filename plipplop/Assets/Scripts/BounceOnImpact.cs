using System.Collections;
using System.Collections.Generic;
using System.Net.Configuration;
using DG.Tweening;
using UnityEngine;

public class BounceOnImpact : MonoBehaviour
{
    public bool scale;
    public bool tilt;

    public float scaleValue;
    public float scaleDuration;
    public float rotationValue;
    public float rotationDuration;

    public AnimationCurve bounceCurve;

    private bool bouncing;

    private void OnCollisionEnter(Collision other)
    {
        Bounce();
    }

    void Bounce()
    {
        if (DOTween.IsTweening(transform) || bouncing)
        {
            return;
        } 

        if (scale)
        {
            //transform.DOPunchScale(Vector3.one * scaleValue, scaleDuration);
            StartCoroutine(BounceScale(scaleValue, scaleDuration));
        }

        if (tilt)
        {
            transform.DOPunchRotation(new Vector3(
                Random.Range(-rotationValue,rotationValue),
                Random.Range(-rotationValue,rotationValue),
                Random.Range(-rotationValue,rotationValue)), 
                rotationDuration);
        }
    }

    IEnumerator BounceScale(float _scale, float _duration)
    {
        bouncing = true;
        
        float timer = 0;

        while (timer < _duration)
        {
            transform.localScale = Vector3.one * bounceCurve.Evaluate(timer / _duration) * _scale;

            timer += Time.deltaTime;
            yield return null;
        }

        bouncing = false;
    }
}
