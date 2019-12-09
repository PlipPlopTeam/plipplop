using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public  enum RotationAxis
{
    None,
    Y,
    All
}

public class MeshVFXPlayer : MonoBehaviour
{
   

    public RotationAxis rotation = RotationAxis.None;
    public bool randomScale;
    public Vector2 randomScaleSize;

    public MeshAnimator animator;

    public string animationName;

    private void OnEnable()
    {
        PlayVFX();
    }

    public void PlayVFX()
    {
        animator.Play(animationName);
        
        if (rotation == RotationAxis.All)
        {
            RotateAll();
        }
        else if(rotation == RotationAxis.Y)
        {
            RotateY();
        }

        if (randomScale)
        {
           ChangeScale(); 
        }
    }

    void RotateY()
    {
        transform.localEulerAngles = new Vector3(0, Random.Range(0f, 360f), 0);
    }

    void RotateAll()
    {
        transform.localEulerAngles = new Vector3(Random.Range(0f, 360f), Random.Range(0f, 360f), Random.Range(0f, 360f));
    }

    void ChangeScale()
    {
        transform.localScale = Vector3.one * Random.Range(randomScaleSize.x, randomScaleSize.y);
    }
}
