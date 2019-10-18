using System.Collections.Generic;
using UnityEngine;

public class Sight : MonoBehaviour
{
    public Vector3 offset;
    public float fieldOfViewAngle = 110f;
    public float range = 5f;

    public GameObject[] Scan(){
        return Scan<GameObject>();
    }

    public T[] Scan<T>()
    {
        List<T> objects = new List<T>();
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
                if(seen) 
                {
                    if(sphereHits[i].transform.gameObject.GetComponent<T>() != null)
                    {
                        objects.Add(sphereHits[i].transform.gameObject.GetComponent<T>());
                    }
                }
            }
        }
        return objects.ToArray();
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color32(255, 215, 0, 255);
        UnityEditor.Handles.color = new Color32(255, 215, 0, 255);
        Vector3 pos = transform.position + offset;
        Vector3 end = pos + transform.forward * range;
        float radius = range * Mathf.Tan((fieldOfViewAngle * Mathf.Deg2Rad)/2f);
        Gizmos.DrawLine(pos, pos + transform.forward * range);
        UnityEditor.Handles.DrawWireDisc(
            end,
            (transform.position - (transform.position + transform.forward * range)).normalized, 
            radius
        );
    }

    void OnValidate()
    {
        if(range < 0f) range = 0f;

        if(fieldOfViewAngle < 1f) fieldOfViewAngle = 1f;
        else if(fieldOfViewAngle > 359f) fieldOfViewAngle = 359f;
    }
#endif
}