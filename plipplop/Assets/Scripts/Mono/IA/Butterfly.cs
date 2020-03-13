using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Butterfly : MonoBehaviour
{
    public Transform butterflyTransform;

    public bool flying;

    private bool flapped;

    public SkinnedMeshRenderer butterflyRenderer;

    public float flappingSpeed;

    public float flappingVariation;

    public float maxFlyTime;
    public float maxLandTime;

    public float flyHeight;

    public float FlyRange;

    private Vector3 currentTarget;
    
    void Start()
    {
        StartFlying();
    }

    void StartFlying()
    {
        butterflyTransform.parent = null;
        
        GetNewTarget();

        flying = true;

        StartCoroutine(Fly());
    }
    
    IEnumerator Fly()
    {
        float _timer = 0;
        while (_timer < maxFlyTime)
        {
            Flap();
            GetNewTarget();
            butterflyTransform.position = Vector3.Lerp(butterflyTransform.position, currentTarget, .1f);
            Orient();
            
            float _time = Random.Range(flappingSpeed - flappingVariation / 2, flappingSpeed + flappingVariation / 2);
            _timer += _time;
            yield return new WaitForSecondsRealtime(_time);
        }
        
        StopFlying();
    }

    IEnumerator GoingHome()
    {
        float _timer = 0;

        while (_timer < 1)
        {

            Flap();
            currentTarget = transform.position;
            butterflyTransform.position = Vector3.Lerp(butterflyTransform.position, currentTarget, _timer);
            Orient();

            _timer += .1f;
            yield return new WaitForSecondsRealtime(.1f);
        }
        
        butterflyTransform.SetParent(transform);
        
        butterflyTransform.localEulerAngles = new Vector3(0,butterflyTransform.localEulerAngles.y,0);
        
        flapped = true;
        Flap();
        
        yield return new WaitForSecondsRealtime(maxLandTime);
        
        StartFlying();
    }

    void StopFlying()
    {
        StartCoroutine(GoingHome());
    }

    void GetNewTarget()
    {
        currentTarget = transform.position + new Vector3(0, Random.Range(flyHeight / 2, flyHeight *2), 0);

        currentTarget += Random.insideUnitSphere * FlyRange;
    }

    void Orient()
    {
        butterflyTransform.forward = (currentTarget - butterflyTransform.position).normalized;
    }

    void Flap()
    {
        butterflyRenderer.SetBlendShapeWeight(0,flapped?0:100);
        flapped = !flapped;
    }
}
