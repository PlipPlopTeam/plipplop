using System.Collections.Generic;
using UnityEngine;

public class Sight : MonoBehaviour
{
	[System.Serializable]
	public class Settings
	{
		public Vector3 offset;
		public float fieldOfViewAngle = 110f;
		public float range = 5f;
	}
	public Settings settings;
	[Range(0f, 1f)] public float multiplier = 1f;

	public GameObject[] Scan(){
        return Scan<GameObject>();
    }

    public T[] Scan<T>()
    {
        List<T> objects = new List<T>();
        Vector3 headPosition = transform.position + settings.offset;

        // Add object in range of being seen
        RaycastHit[] sphereHits;
        sphereHits = Physics.SphereCastAll(headPosition, settings.range * multiplier, transform.forward, settings.range);
        for(int i = 0; i < sphereHits.Length; i++)
        {
            // Checks if the object is in front of the sight
            Vector3 direction = sphereHits[i].transform.position - headPosition;
            float angle = Vector3.Angle(direction, transform.forward);
            if(angle < settings.fieldOfViewAngle * 0.5f * multiplier)
            {

                // Check if an object is hiding the seen object from the sight
                RaycastHit[] rayHits;
                rayHits = Physics.RaycastAll(headPosition, direction, settings.range * multiplier, 5, QueryTriggerInteraction.Ignore);

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
        Vector3 pos = transform.position + settings.offset;
        Vector3 end = pos + transform.forward * settings.range;
        float radius = settings.range * Mathf.Tan((settings.fieldOfViewAngle * Mathf.Deg2Rad)/2f);
        Gizmos.DrawLine(pos, pos + transform.forward * settings.range);
        UnityEditor.Handles.DrawWireDisc(
            end,
            (transform.position - (transform.position + transform.forward * settings.range)).normalized, 
            radius
        );
    }

    void OnValidate()
    {
		if (settings == null) return;

        if(settings.range < 0f) settings.range = 0f;
        if(settings.fieldOfViewAngle < 1f) settings.fieldOfViewAngle = 1f;
        else if(settings.fieldOfViewAngle > 359f) settings.fieldOfViewAngle = 359f;
    }
#endif
}