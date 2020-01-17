using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CinematicVolume : Volume
{

    [Header("Specific options")]
    public SpielbergAssistant cinematic;
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
        cinematic.Play();
        Destroy(gameObject);
    }

    public override void OnPlayerExit(Controller player)
    {

    }
}
