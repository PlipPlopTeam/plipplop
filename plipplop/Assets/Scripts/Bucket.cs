using System.Collections.Generic;
using UnityEngine;

public class Bucket : Controller
{
	Dictionary<int, float> slowdown = new Dictionary<int, float>();
	int currentSlow = -1;
	Container container;
	public FoodData debugFoodItem;
    public int itemCount { get { return container.GetItemCount(); } }

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
			this.Slow(container.GetItemCount());
		};

		container.onItemStored += () =>
		{
			this.Slow(container.GetItemCount());
		};

		slowdown.Add(0, 0.9f);        
		slowdown.Add(1, 0.85f);
		slowdown.Add(2, 0.70f);
		slowdown.Add(3, 0.55f);
		slowdown.Add(4, 0.40f);
		slowdown.Add(5, 0.25f);
	}

	public void Slow(int itemStored)
	{

		if(slowdown.TryGetValue(itemStored, out float slow))
		{
			if (this.currentSlow != -1) this.locomotion.RemoveModifier(this.currentSlow);
			currentSlow = locomotion.AddModifier(slow);
		}
	}
}
