using System.Collections.Generic;
using UnityEngine;

public class Bucket : Controller
{
	private Dictionary<int, float> slowdown = new Dictionary<int, float>();
	private int currentSlow = -1;
	private Container container;
	public FoodData debugFoodItem;

	[ContextMenu("DebugSpawnFood")]
	public void DebugSpawnFood()
	{
		Food fo = new GameObject().AddComponent<Food>();
		fo.Create(debugFoodItem);
		container.Store(fo);
	}

	override internal void Awake()
    {
		base.Awake();

		container = GetComponent<Container>();
		if (container == null)
		{
			Destroy(gameObject);
			return;
		}

		container.onItemStored += () =>
		{
			if (this.currentSlow != -1) this.locomotion.RemoveModifier(this.currentSlow);
			this.Slow(container.GetItemCount());
		};

		slowdown.Add(0, 0f);
		slowdown.Add(1, 15f);
		slowdown.Add(2, 30f);
		slowdown.Add(3, 45f);
		slowdown.Add(4, 60f);
		slowdown.Add(5, 75f);
	}

	public void Slow(int itemStored)
	{
		float slow = 0f;
		if (slowdown.TryGetValue(itemStored, out slow))
		{
			currentSlow = locomotion.AddModifier(slow);
		}
	}
}
