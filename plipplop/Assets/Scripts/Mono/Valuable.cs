using UnityEngine;
using System.Collections.Generic;

public class Valuable : Item, INoticeable, ICarryable
{
    [HideInInspector] public Vector3 origin;
    [Header("Settings")]
    public float weight = 1f;
    public float distanceThreshold = 2f;
    public bool hidden = false;

	void Start()
    {
        origin = transform.position;
    }
	public void Notice()
    {
        // Does things..
    }
    public bool IsVisible()
    {
		return !hidden;// && !IsSurroundedBySameItem();
    }
    public void SetVisible(bool value)
    {
        hidden = value;
    }
    public bool HasMoved()
    {
        return Vector3.Distance(origin, transform.position) > distanceThreshold;
    }

	public bool IsSurroundedBySameItem()
	{
		MeshFilter mf = gameObject.GetComponentInChildren<MeshFilter>();
		if (mf != null)
		{
			Mesh m = mf.mesh;

			Collider[] surrounders = Physics.OverlapSphere(transform.position, 3f);
			int count = 0;
			foreach(Collider c in surrounders)
			{
				MeshFilter cmf = c.transform.gameObject.GetComponentInChildren<MeshFilter>();
				if (cmf != null && m == cmf.mesh) count++;
			}

			if (count >= 2) return true;
			else return false;
		}
		else return false;
	}
}
