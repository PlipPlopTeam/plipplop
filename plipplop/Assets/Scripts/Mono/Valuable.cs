using UnityEngine;
using System.Collections.Generic;

public class Valuable : Item, INoticeable, ICarryable
{
    [HideInInspector] public Vector3 origin;
	public List<Transform> placesToBeSorted = new List<Transform>();
    [Header("Valuable")]
    public float distanceThreshold = 2f;
    public bool hidden = false;
	public bool active = true;

	new public virtual void Awake()
    {
		if(placesToBeSorted.Count > 0)
		{
			origin = placesToBeSorted.PickRandom().position;
		}
		else
		{
			origin = transform.position;
		}
	}
	public virtual void Notice()
    {
        // Does things..
    }
	public virtual bool IsVisible()
    {
		return !hidden && active/* && !IsSurroundedBySameItem()*/;
    }
	public virtual void SetVisible(bool value)
    {
        hidden = value;
    }
	public virtual bool HasMoved()
    {
        return Vector3.Distance(origin, transform.position) > distanceThreshold;
    }
	public virtual bool IsSurroundedBySameItem()
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
