using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crab : MonoBehaviour
{
    public GameObject knife;
    public Transform knifeTransform;

    public float knifeChance = .1f;

    void Start()
    {
        if (Random.Range(0f, 1f) < knifeChance)
        {
            knife = Instantiate(knife, knifeTransform);
        }
    }

}
