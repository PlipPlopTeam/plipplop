using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CinematicVolume : Volume
{

    [Header("Specific options")]
    public string cinematic;
    public bool destroyOnCinemaStart = true;
    public bool isInvisible = true;
    [HideInInspector] public GameObject visual;
    public Material material;

    private void Start()
    {
        UpdateSize();
        if (isInvisible) return;

        visual = GameObject.CreatePrimitive(PrimitiveType.Cube);
        visual.transform.localScale = GetSize();
        visual.transform.parent = transform;
        visual.transform.localPosition = Vector3.zero;
        if (material) visual.GetComponent<Renderer>().material = material;
        Destroy(visual.GetComponent<Collider>());
    }

    public override void OnPlayerEnter(Controller player)
    {
        try {
            Spielberg.PlayCinematic("cine_test_1");

            if (destroyOnCinemaStart) Destroy(gameObject);
        }
        catch (NullReferenceException) {
            Debug.LogError("NO CINEMATIC WAS TRIGGERED because no cinematic was linked to VOLUME " + name);
        }
    }

    public override void OnPlayerExit(Controller player)
    {

    }
}
