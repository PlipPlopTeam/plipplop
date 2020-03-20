using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterVolume : Volume
{

    [Header("Specific options")]
    public bool isInvisible = false;
    public GameObject customVisual;
    public Material material;
    public Mesh zoneMesh;

    ImmergedBodies objectsInWater = new ImmergedBodies();

    // PARAMETERS
    float tractionToSurface = 4f;   // The lower this value is, the more objects will slowdown when approaching the surface
    float waterForce = 50f;        // The force that pushes objects upward
    float maxUpSpeed = 6f;          // The maximum upward speed of immerged objects
    float waterline = 1F;         // Objects will stop a bit below water instead of perfectly at the surface. This is the distance from surface (downwards)
    float artificialDrag = 2f;      // Objects will lerp to zero speed using this value to simulate a drag      
    float aboveWaterSpeedReduction = 0.4f;   // Upon exiting water the vertical velocity of the object is altered to avoid bouncing
    float velocitySplash = 1f;   // Upon exiting water the vertical velocity of the object is altered to avoid bouncing

    private void Start()
    {
        if (isInvisible) return;
        if (!customVisual) customVisual = GameObject.CreatePrimitive(PrimitiveType.Cube);
        if (zoneMesh) customVisual.GetComponent<MeshFilter>().mesh = zoneMesh;

        customVisual.transform.localScale = GetSize();
        customVisual.GetComponent<Renderer>().material = new Material(material);
        customVisual.transform.parent = transform;
        customVisual.transform.localPosition = Vector3.zero;
        Destroy(customVisual.gameObject.GetComponent<Collider>());
    }

     public override void OnObjectEnter(Collider obj)
    {
        var rb = obj.GetComponent<Rigidbody>();
        if (rb) {
            bool isNew = objectsInWater.Add(rb, obj);
            var con = obj.GetComponent<Controller>();
            if (con && isNew) {
                con.SetUnderwater();
            }
            if (isNew)
            {
                // rentre
                //SoundPlayer.PlayAtPosition("sfx_splash", rb.transform.position);
                // TODO : Integrate GameEffect
                if(rb.velocity.magnitude >= velocitySplash)
                {
                    Pyromancer.PlayGameEffect("gfx_splash", obj.transform.position);
                }
            }
        }
    }

    public override void OnObjectExit(Collider obj)
    {
        var rb = obj.GetComponent<Rigidbody>();
        if (rb) {
            var outOfWater = objectsInWater.Remove(obj);

            if (outOfWater) {
                objectsInWater.Remove(rb);

                // sort

                rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y * aboveWaterSpeedReduction, rb.velocity.z);

                var con = rb.GetComponent<Controller>();
                if (con) {
                    con.SetOverwater();
                }
            }
        }
    }

    public override void OnPlayerEnter(Controller player)
    {

    }

    public override void OnPlayerExit(Controller player)
    {

    }

    private void FixedUpdate()
    {
        foreach(var b in objectsInWater) 
        {
            if (b.rigidbody == null) continue;

            var rb = b.rigidbody;
            var force = 10f / Mathf.Max(1f, b.rigidbody.mass);
            var customWaterline = waterline;
            var cb = rb.GetComponent<CustomBuyoyancy>();
            if (cb) {
                force = cb.buyoyancy;
                customWaterline = cb.waterline;
            }

            //Reducing force as the object is surfacing to avoid permanent wobbling
            var distanceBelowSurface = - (rb.transform.position - transform.position - transform.up * (height/2f)).y + customWaterline;
            distanceBelowSurface = distanceBelowSurface * tractionToSurface;


            // If I cannot float, I shall sink
            if (force < 1f) {
                force *= -1f;
                force -= 1f;
                distanceBelowSurface = - force + Mathf.Abs(distanceBelowSurface);
            }

            // Mean of all positions of colliders
            Vector3 forcePosition = Vector3.zero;
            foreach(var col in b.colliders) {
                forcePosition += col.bounds.center;
            }
            forcePosition /= b.colliders.Count;

            rb.AddForceAtPosition(
                transform.up * (force) * waterForce * Mathf.Clamp(distanceBelowSurface, -1f, 1f) * Time.fixedDeltaTime,
                forcePosition,
                ForceMode.Acceleration
            );

            rb.velocity = new Vector3(rb.velocity.x, 
                Mathf.Lerp(
                    Mathf.Min(rb.velocity.y, maxUpSpeed),
                    0f,
                    artificialDrag * Time.fixedDeltaTime
                ),
                rb.velocity.z
            );
        }
    }

#if UNITY_EDITOR
    internal override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
        UnityEditor.Handles.Label(
            transform.position + Vector3.up,
            objectsInWater.Count.ToString()
        );
    }
#endif
}
