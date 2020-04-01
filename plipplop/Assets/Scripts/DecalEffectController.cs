using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.HDPipeline;

public class DecalEffectController : MonoBehaviour, IVisualEffectController
{
    public bool alive;

    private DecalProjectorComponent decal;

    private void Awake()
    {
        decal = GetComponent<DecalProjectorComponent>();
        alive = true;
        StartCoroutine(Fade());
    }

    public void Activate()
    {
        gameObject.SetActive(true);
        
    }

    public void SetPosition(Vector3 v)
    {
        transform.position = v;
    }

    public void SetLocalPosition(Vector3 v)
    {
        transform.localPosition = v;
    }

    public void Attach(Transform p)
    {
        transform.parent = p;
    }

    public void Pause()
    {
        
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }

    public void Reset()
    {
        StopAllCoroutines();
        alive = true;
        StartCoroutine(Fade());
    }

    public bool IsAlive()
    {
        return alive;
    }

    IEnumerator Fade()
     {
         float timer = 0;

         while (timer < 1)
         {
             decal.fadeFactor = 1f - timer;
             
             timer += Time.deltaTime /3f;
             yield return null;
         }

         alive = false;

         gameObject.SetActive(false);
     }
}
