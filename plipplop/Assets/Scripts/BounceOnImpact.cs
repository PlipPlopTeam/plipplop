using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

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

    public Transform transformToMove;

    public float chanceToDetachObject = .5f;
    
    public List<GameObject> objectsToDetach;
    
    public string gfxToPlay = "gfx_bush";
    public Transform gfxSpawnPoint;
        
    private void Start()
    {
        if (transformToMove == null)
        {
            transformToMove = transform;
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        Bounce();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger == false)
        {
            Bounce();
        }
    }

    void Bounce()
    {
        if (DOTween.IsTweening(transform) || bouncing)
        {
            return;
        }

        if (gfxToPlay != null)
        {
            if (gfxSpawnPoint != null)
            {
                Pyromancer.PlayGameEffect(gfxToPlay, gfxSpawnPoint.position);
            }
            else
            {
                Pyromancer.PlayGameEffect(gfxToPlay, transform.position);
            }
        }

        if (objectsToDetach.Count > 0)
        {
            if (Random.Range(0f, 1f) < chanceToDetachObject)
            {
                DetachObject();
            }
        }
        
        
        if (scale)
        {
            //transform.DOPunchScale(Vector3.one * scaleValue, scaleDuration);
            StartCoroutine(BounceScale(scaleValue, scaleDuration));
        }

        if (tilt)
        {
            transformToMove.DOPunchRotation(new Vector3(
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
            transformToMove.localScale = Vector3.one * bounceCurve.Evaluate(timer / _duration) * _scale;

            timer += Time.deltaTime;
            yield return null;
        }

        bouncing = false;
    }

    void DetachObject()
    {
        GameObject _g = objectsToDetach[0];
        _g.transform.parent = null;
        Rigidbody _rb = _g.GetComponent<Rigidbody>();

        if (!_rb)
        {
            _rb = _g.AddComponent<Rigidbody>();
        }
        else
        {
            _rb.isKinematic = false;
        }
        
        _rb.AddForce(Random.insideUnitSphere);

        objectsToDetach.Remove(_g);
    }
    
}
