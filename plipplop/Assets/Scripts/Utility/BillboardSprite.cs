using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillboardSprite : MonoBehaviour
{
    private Transform target;

    void Start()
    {

        if (Game.i)
        {
            target = Game.i.aperture.currentCamera.transform;
        }else
        {
            target = Camera.main.transform;
        }
    }

    // Update is called once per frame
    void Update()
    {
        transform.forward = -(transform.position - target.position).normalized;
    }
}
