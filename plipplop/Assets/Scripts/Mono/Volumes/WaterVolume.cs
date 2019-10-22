using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterVolume : Volume
{

    [Header("Specific options")]
    public bool isInvisible = false;
    public GameObject customVisual;
    public Material material;

    public float waterForce = 10f;
    List<Rigidbody> objectsInWater = new List<Rigidbody>();
    float tractionToSurface = 4f;

    private void Start()
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
        if (rb) {
            if (objectsInWater.Contains(rb)) return;

            objectsInWater.Add(rb);

            var con = obj.GetComponent<Controller>();
            if (con) {
                con.SetUnderwater();
            }
        }
    }

    public override void OnObjectExit(GameObject obj)
    {
        var rb = obj.GetComponent<Rigidbody>();
        if (rb) {
            objectsInWater.RemoveAll(o => o == rb);
            var con = rb.GetComponent<Controller>();
            if (con) {
                con.SetOverwater();
            }
        }
    }

    public override void OnPlayerEnter(Controller player)
    {
        //throw new System.NotImplementedException();
    }

    public override void OnPlayerExit(Controller player)
    {
        //throw new System.NotImplementedException();
    }

    private void FixedUpdate()
    {
        foreach(var b in objectsInWater) {
            var force = 10f / b.mass;
            var cb = b.GetComponent<CustomBuyoyancy>();
            if (cb) {
                force = cb.buyoyancy;
            }

            //Reducing force as the object is surfacing to avoid permanent wobbling
            var distanceBelowSurface = - (b.transform.position - transform.position - transform.up * (height/2f)).y;
            distanceBelowSurface = Mathf.Max(distanceBelowSurface, 0f) * tractionToSurface;

            b.AddForce(
                transform.up * (1 / force) * waterForce * Mathf.Clamp01(distanceBelowSurface) * Time.fixedDeltaTime,
                ForceMode.Acceleration
            );
        }
    }
}
