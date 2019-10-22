using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterVolume : Volume
{

    [Header("Specific options")]
    public bool isInvisible = false;
    public GameObject customVisual;
    public Material material;

    List<Rigidbody> objectsInWater = new List<Rigidbody>();


    private void Awake()
    {
        if (!customVisual) customVisual = GameObject.CreatePrimitive(PrimitiveType.Cube);

        customVisual.transform.localScale = GetSize();
        customVisual.GetComponent<Renderer>().material = new Material(material);
        customVisual.transform.parent = transform;
        customVisual.transform.localPosition = Vector3.zero;
        Destroy(customVisual.gameObject.GetComponent<Collider>());
    }


    public override void OnObjectEnter(GameObject obj)
    {
        var rb = obj.GetComponent<Rigidbody>();
        if (rb) objectsInWater.Add(rb);
    }

    public override void OnObjectExit(GameObject obj)
    {
        var rb = obj.GetComponent<Rigidbody>();
        if (rb) objectsInWater.RemoveAll(o=>o == rb);
    }

    public override void OnPlayerEnter(Controller player)
    {
        //throw new System.NotImplementedException();
    }

    public override void OnPlayerExit(Controller player)
    {
        //throw new System.NotImplementedException();
    }
}
