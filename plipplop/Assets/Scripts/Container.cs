using System.Collections.Generic;
using UnityEngine;

public class Container : Item
{
	[Header("Settings")]
	public bool showStoredItem = true;
	public bool randomizeStoreItemRotation = true;
	public Vector3 storedOffset;
	public float storedRange;
	public List<Item.EType> madeFor = new List<Item.EType>();

	public bool emptyIfReversed = true;
	public float reverseAngle = 45f;
	private List<Item> items = new List<Item>();
	public System.Action onItemStored;

	public int GetItemCount()
	{
		return items.Count;
	}

	public virtual void Store(Item item)
	{
		item.Carry();
		item.transform.SetParent(transform);
		item.transform.localPosition = Vector3.zero;
		items.Add(item);
		if(showStoredItem)
		{
			if (randomizeStoreItemRotation) item.transform.localEulerAngles = new Vector3(Random.Range(0f, 360f), Random.Range(0f, 360f), Random.Range(0f, 360f));
			item.transform.localPosition = storedOffset + Geometry.GetRandomPointInSphere(storedRange);
		}
		else
		{
			item.visuals.SetActive(false);
		}
	}

	public bool IsMadeFor(Item item)
	{
		return item != null && madeFor.Contains(item.type);
	}

	public virtual void Update()
	{
		if(emptyIfReversed 
		&& Vector3.Angle(transform.up, Vector3.up) >= reverseAngle
		&& items.Count > 0f)
		{
			Empty();
		}
	}

	public virtual void Remove(Item item)
	{
		Free(item);
		items.Remove(item);
	}
	public virtual void Remove(int index)
	{
		if (index < 0 || index >= items.Count) return;
		Free(items[index]);
		items.RemoveAt(index);
	}

	public virtual void Free(Item item)
	{
		item.visuals.SetActive(true);
		item.Drop();
		item.transform.SetParent(null);
	}

	public virtual void Empty()
	{
		foreach(Item i in items) Free(i);
		items.Clear();
	}

#if UNITY_EDITOR
	private void OnDrawGizmosSelected()
	{
		if(showStoredItem) Gizmos.DrawWireSphere(transform.position + storedOffset, storedRange);

		if(emptyIfReversed)
		{
			float a = 90f - reverseAngle;
			Vector3 dir = new Vector3(0f, Mathf.Sin(Mathf.Deg2Rad * a), Mathf.Cos(Mathf.Deg2Rad * a));
			Gizmos.DrawLine(transform.position, transform.position + dir);
		}
	}
#endif
}
