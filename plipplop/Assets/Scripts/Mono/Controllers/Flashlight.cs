using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flashlight : Vanilla
{
    [Header("FLASHLIGHT")]
    [Header("References")]
    public GameObject fakeLightCylinder;
    public Light spotLight;
    [Header("Settings")]
    public float speedToLight = 1f;
    public float maxCylinderSize = 5f;
    public float incrementalSpeed = 1f;
    public float decrementalSpeed = 1f;

    float intensity = 0f;

    internal override void Update()
    {
        base.Update();

        if(IsPossessed())
        {
            if(rigidbody.velocity.magnitude > speedToLight) intensity += Time.deltaTime * incrementalSpeed;
            else if(intensity > 0) intensity -= Time.deltaTime * decrementalSpeed;

            float size = intensity;
            if(size > maxCylinderSize) size = maxCylinderSize;
            else if(size < 0) size = 0;
            fakeLightCylinder.transform.localScale = new Vector3(size, fakeLightCylinder.transform.localScale.y, fakeLightCylinder.transform.localScale.z);
        }
    }
}
