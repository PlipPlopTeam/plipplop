using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindVolume : Volume
{

    [Header("Specific options")]
    public bool isInvisible = false;
    public bool constantPushing = true;
    [Range(0.001f, 1f)] public float minimumProportionalPushFactor = 0.01f;
    [Range(0.1f, 100f)]public float windForce = 10f;         
    
    ImmergedBodies objectsInVolume = new ImmergedBodies();

    private void Start()
    {
        if (isInvisible) return;
    }


    public override void OnObjectEnter(Collider obj)
    {
        var rb = obj.GetComponent<Rigidbody>();
        if (rb) {
            objectsInVolume.Add(rb, obj);
        }
    }

    public override void OnObjectExit(Collider obj)
    {
        var rb = obj.GetComponent<Rigidbody>();
        if (rb) {

            var outOfWind = objectsInVolume.Remove(obj);

            if (outOfWind) {
                objectsInVolume.Remove(rb);
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
        foreach(var b in objectsInVolume) {
            var rb = b.rigidbody;

            //Reducing force as the object is surfacing to avoid permanent wobbling
            var tractionMultiplier = 1f;

            if (!constantPushing) {
                var positionInWind = transform.InverseTransformPoint(rb.transform.position).z - length/2f;
                var factor = Mathf.Max(minimumProportionalPushFactor, (-positionInWind)/length);
                tractionMultiplier = 1 / (1f - factor);
                Mathf.Max(tractionMultiplier, 0f);
            }


            // Mean of all positions of colliders
            Vector3 forcePosition = Vector3.zero;
            foreach (var col in b.colliders) {
                forcePosition += col.bounds.center;
            }
            forcePosition *= 1 / b.colliders.Count;


            rb.AddForceAtPosition(
                transform.forward * (windForce*1000f) * tractionMultiplier * Time.fixedDeltaTime,
                transform.position + forcePosition,
                ForceMode.Acceleration
            );
        }
    }

#if UNITY_EDITOR
	new void OnDrawGizmos()
    {
        base.OnDrawGizmos();
        Gizmos.matrix = transform.localToWorldMatrix;

        int unit = 4;
        var arrowLength = 0.25f*unit;
        var shift = ((Time.realtimeSinceStartup/50F) * windForce) % 1f;

        for (var x = unit/2f; x < width; x+= unit) {

            for (var y = unit/2f; y < height; y+= unit) {

                for (var z = unit/2f; z < length; z+= unit) {

                    var sizeFactor = 1f;

                    if (!constantPushing) {
                        sizeFactor = z / length;
                    }

                   // Debug.Log(sizeFactor);

                    var tx = - width / 2f + x;
                    var ty = - height / 2f + y;
                    var tz = - length / 2f + z + shift*arrowLength - arrowLength/2f;
                    var lz1 = Mathf.Clamp(tz - arrowLength * sizeFactor,  - length / 2f, length / 2f);
                    var lz2 = Mathf.Clamp(tz + arrowLength * sizeFactor,  - length / 2f, length / 2f);

                    Gizmos.DrawLine(
                        new Vector3(tx, ty, lz1), 
                        new Vector3(tx, ty, lz2)
                    );
                    Gizmos.DrawLine(
                        new Vector3(tx, ty, lz2),
                        new Vector3(tx + arrowLength* sizeFactor, ty, (lz1 + lz2) / 2f)
                    );
                    Gizmos.DrawLine(
                        new Vector3(tx, ty, lz2),
                        new Vector3(tx - arrowLength* sizeFactor, ty, (lz1+lz2)/2f)
                    );;

                }
            }
        }
    } 
#endif
}
