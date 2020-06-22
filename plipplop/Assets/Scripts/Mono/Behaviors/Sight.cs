using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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
	public Transform origin;
	[Range(0f, 1f)] public float multiplier = 1f;
	const float refresh = 0.25f;


	public Dictionary<string, float> memory = new Dictionary<string, float>();

	public void Awake()
	{
		if (origin == null) origin = transform;
	}

	public GameObject[] Scan()
	{
        return Scan<GameObject>();
    }

	public void Update()
	{
		for (int i = 0; i < memory.Count; i++)
		{
			if (memory.ElementAt(i).Value > 0f) memory[memory.ElementAt(i).Key] -= Time.deltaTime;
			else memory.Remove(memory.ElementAt(i).Key);
		}
	}

	public T[] Scan<T>()
    {
		if (memory.ContainsKey(typeof(T).ToString())) return new List<T>().ToArray();
		else memory.Add(typeof(T).ToString(), refresh);

        List<T> objects = new List<T>();
        Vector3 headPosition = origin.position + settings.offset;

        // Add object in range of being seen
        RaycastHit[] sphereHits;
        sphereHits = Physics.SphereCastAll(headPosition, settings.range * multiplier, origin.forward, settings.range);
        for(int i = 0; i < sphereHits.Length; i++)
        {
			if(sphereHits[i].transform.gameObject.GetComponent<T>() != null && See(sphereHits[i].transform.gameObject))
			{
				objects.Add(sphereHits[i].transform.gameObject.GetComponent<T>());
			}
        }
        return objects.ToArray();
    }

	public bool See(GameObject o)
	{
		Vector3 headPosition = origin.position + settings.offset;
		Vector3 direction = o.transform.position - headPosition;
		float angle = Vector3.Angle(direction, origin.forward);
		if (angle < settings.fieldOfViewAngle * 0.5f * multiplier)
		{
			bool seen = true;
			foreach (RaycastHit hit in Physics.RaycastAll(headPosition, direction, settings.range * multiplier, 5, QueryTriggerInteraction.Ignore))
			{
				if (hit.transform != origin && !o.transform.IsYourselfCheck(hit.transform))
				{
					//if (Vector3.Distance(headPosition, o.transform.position) > Vector3.Distance(headPosition, hit.point)) seen = false;
					seen = false;
				}
			}

			return seen;
		}
		else return false;
	}

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
		Transform o = origin;
		if (o == null) o = transform;

        Gizmos.color = new Color32(255, 215, 0, 255);
        UnityEditor.Handles.color = new Color32(255, 215, 0, 255);
        Vector3 pos = o.position + settings.offset;
        Vector3 end = pos + o.forward * settings.range;
        float radius = settings.range * Mathf.Tan((settings.fieldOfViewAngle * Mathf.Deg2Rad)/2f);
        Gizmos.DrawLine(pos, pos + o.forward * settings.range);
        UnityEditor.Handles.DrawWireDisc(
            end,
            (o.position - (o.position + o.forward * settings.range)).normalized, 
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