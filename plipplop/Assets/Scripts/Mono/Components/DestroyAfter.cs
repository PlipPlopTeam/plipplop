using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfter : MonoBehaviour
{
    public float lifespan = 5f;

    float createTime = 0f;


    private void Awake()
    {
        createTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if (createTime + lifespan < Time.time) {
            DestroyImmediate(gameObject);
        }
    }
}
