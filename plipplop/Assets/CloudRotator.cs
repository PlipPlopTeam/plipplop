using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudRotator : MonoBehaviour
{
    public float speed;

    public float fps;
    
    private void Start()
    {
        StartCoroutine(Rotation());
    }

    IEnumerator Rotation()
    {
        while (true)
        {
            transform.Rotate(Vector3.up * speed);
            yield return new WaitForSecondsRealtime(1 / fps);
        }
    }
}
