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
	public System.Action onItemRemoved;

	public override void Awake()
	{
        base.Awake();
		Game.i.aiZone.Register(this);
	}

	public void Start()
	{
		Item[] itemsInMe = gameObject.GetComponentsInChildren<Item>();
		foreach (Item i in itemsInMe)
		{
			if (i.transform != this.transform)
			{
				Store(i);
			}
		}
	}

	public int GetItemCount()
	{
		return items.Count;
	}

	public virtual void Store(Item item)
	{
		item.Carry();
		item.transform.SetParent(visuals.transform);
		item.transform.localPosition = Vector3.zero;
		items.Add(item);

		SoundPlayer.PlayAtPosition("sfx_item_bump", transform.position, 1f, true);

		if (showStoredItem)
		{
			if (randomizeStoreItemRotation) item.transform.localEulerAngles = new Vector3(Random.Range(0f, 360f), Random.Range(0f, 360f), Random.Range(0f, 360f));
			item.transform.localPosition = storedOffset + Geometry.GetRandomPointInSphere(storedRange);
		}
		else
		{
			item.visuals.SetActive(false);
		}

		if (onItemStored != null) onItemStored.Invoke();
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
		if(item.visuals != null) item.visuals.SetActive(true);
		item.Drop();
		item.transform.SetParent(null);
		if (onItemRemoved != null) onItemRemoved.Invoke();
	}

	public virtual void Empty()
	{
		foreach(Item i in items) Remove(i);
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
