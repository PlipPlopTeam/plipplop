using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualEffectController : MonoBehaviour
{
    ParticleSystem shuriken;

    private void Awake()
    {
        shuriken = GetComponent<ParticleSystem>();
    }

    public void SetPosition(Vector3 v)
    {
        gameObject.transform.position = v;
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
        return shuriken.IsAlive();
    }

    private void Update()
    {
        if (!IsAlive()) {
            gameObject.SetActive(false);
        }
    }
}
