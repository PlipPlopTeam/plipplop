using System.Collections.Generic;
using UnityEngine;

public class Sight : MonoBehaviour
{
    public Vector3 offset;
    public float fieldOfViewAngle = 110f;
    public float range = 5f;

    public GameObject[] Scan()
    {
        List<GameObject> objects = new List<GameObject>();
        Vector3 headPosition = transform.position + offset;

        // Add object in range of being seen
        RaycastHit[] sphereHits;
        sphereHits = Physics.SphereCastAll(headPosition, range, transform.forward, range);

        for(int i = 0; i < sphereHits.Length; i++)
        {
            // Checks if the object is in front of the sight
            Vector3 direction = sphereHits[i].transform.position - headPosition;
            float angle = Vector3.Angle(direction, transform.forward);
            if(angle < fieldOfViewAngle * 0.5f)
            {

                // Check if an object is hiding the seen object from the sight
                RaycastHit[] rayHits;
                rayHits = Physics.RaycastAll(headPosition, direction, range, 5, QueryTriggerInteraction.Ignore);

                bool seen = true;
                foreach(RaycastHit hit in rayHits)
                {
                    if(hit.transform != transform && hit.transform != sphereHits[i].transform) 
                    {
                        if(Vector3.Distance(headPosition, sphereHits[i].transform.position) > Vector3.Distance(headPosition, hit.point)) seen = false;
                    }
                }
                if(seen) objects.Add(sphereHits[i].transform.gameObject);
            }
        }
        return objects.ToArray();
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color32(173, 216, 230, 255);

        Quaternion right = Quaternion.AngleAxis(-fieldOfViewAngle/2, transform.up);
        Quaternion left = Quaternion.AngleAxis(fieldOfViewAngle/2, transform.up);
        Quaternion up = Quaternion.AngleAxis(-fieldOfViewAngle/2, transform.right);
        Quaternion down = Quaternion.AngleAxis(fieldOfViewAngle/2, transform.right);

        Gizmos.DrawLine(transform.position + offset, right * transform.forward * range + offset);
        Gizmos.DrawLine(transform.position + offset, left * transform.forward * range + offset);
        Gizmos.DrawLine(transform.position + offset, up * transform.forward * range + offset);
        Gizmos.DrawLine(transform.position + offset, down * transform.forward * range + offset);

        Vector3 end = ((up * transform.forward * range) + (down * transform.forward * range)) / 2f;
        UnityEditor.Handles.DrawWireDisc(
            end + offset,
            (transform.position - (transform.position + transform.forward * range)).normalized, 
            Vector3.Distance(end, up * transform.forward * range)
        );
    }
}