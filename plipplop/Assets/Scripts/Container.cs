using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Container : MonoBehaviour
{
	[Header("Settings")]
	public bool showStockedItem;
	public Vector3 stockOffset;
	public float stockRange;
	public List<Item> items = new List<Item>();

	public void Stock(Item item)
	{
		item.Carry();
		item.transform.SetParent(transform);
		item.transform.localPosition = Vector3.zero;
		items.Add(item);
		if(showStockedItem)
		{
			item.transform.localPosition = transform.position + stockOffset + Geometry.GetRandomPointInSphere(stockRange);
		}
		else
		{
			item.visuals.SetActive(false);
		}
	}

	public void Empty()
	{
		foreach(Item i in items)
		{
			i.visuals.SetActive(true);
			i.Drop();
			i.transform.SetParent(null);
		}
		items.Clear();
	}

#if UNITY_EDITOR
	private void OnDrawGizmosSelected()
	{
		Gizmos.DrawWireSphere(transform.position + stockOffset, stockRange);
	}
#endif
}
