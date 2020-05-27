using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShurikenEffectController : MonoBehaviour,IVisualEffectController
{
    ParticleSystem shuriken;

    private void Awake()
    {
        shuriken = GetComponent<ParticleSystem>();
    }

    public void Activate()
    {
        gameObject.SetActive(true);
    }

    public void SetPosition(Vector3 v)
    {
        gameObject.transform.position = v;
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
        shuriken.Pause();
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }

    public void Reset()
    {
        shuriken.Clear();
    }

    public bool IsAlive()
    {
        return shuriken != null && shuriken.IsAlive();
    }

    private void Update()
    {
        if (!IsAlive()) {
            gameObject.SetActive(false);
        }
    }
}
