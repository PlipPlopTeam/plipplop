using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Kite : Item
{
    public LineRenderer line;

    public float maxHeight;

    public float maxRange;

    public Vector3 WindDirection;
    public float maxWindDistance;

    public float updateSpeed;
    
    public bool flying;

    public AnimationCurve lineCurve;

    public int linePointAmount;
    


    public void StartFly()
    {
        if (flying)
        {
            return;
        }
        StartCoroutine(Fly());
    }

    public void StopFly()
    {
        StartCoroutine(FlyDown());
    }

    IEnumerator Fly()
    {
        flying = true;
        visuals.transform.parent = null;
        StartCoroutine(UpdateLine());
        
        while (flying)
        {
            SetPosition();
            
            SetRotation();
            
            yield return new WaitForSecondsRealtime(updateSpeed);
        }
        
        
    }

    IEnumerator FlyDown()
    {
        flying = false;
        
        float _y = 0;

        while (_y < 1)
        {
            visuals.transform.position = Vector3.Lerp(visuals.transform.position, transform.position, _y);
            DrawLine();
            _y += .2f;
            yield return new WaitForSecondsRealtime(.2f);
        }
        
        visuals.transform.SetParent(transform);

        visuals.transform.localPosition = Vector3.zero;
        visuals.transform.localEulerAngles = Vector3.zero;

        line.enabled = false;
        
    }

    IEnumerator UpdateLine()
    {
        line.enabled = true;
        while (flying)
        {
            DrawLine();
            yield return null;
        }
    }

    void SetPosition()
    {
        visuals.transform.position = Vector3.Lerp(visuals.transform.position,transform.position + Vector3.up * maxHeight + WindDirection * maxWindDistance +
                                                                           Random.insideUnitSphere * maxRange,.5f);
    }

    void SetRotation()
    {
        visuals.transform.up = (visuals.transform.position - transform.position).normalized;
            
        //need rotation toward transform.position
    }

    void DrawLine()
    {
        for (int i = 0; i < linePointAmount; i++)
        {
            line.SetPosition(i, new Vector3(Mathf.Lerp(transform.position.x, visuals.transform.position.x, (float)i/(linePointAmount-1)),
                                            Mathf.Lerp(transform.position.y, visuals.transform.position.y, lineCurve.Evaluate((float)i/(linePointAmount-1))),
                                            Mathf.Lerp(transform.position.z, visuals.transform.position.z, (float)i/(linePointAmount-1))) );
        }
    }
}
