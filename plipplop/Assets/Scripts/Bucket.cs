using System.Collections.Generic;
using UnityEngine;

public class Bucket : Controller
{
	Dictionary<int, float> slowdown = new Dictionary<int, float>();
	int currentSlow = -1;
	Container container;
	ParticleSystem sweat;
    public int itemCount { get { return container.GetItemCount(); } }

	override internal void Awake()
    {
		base.Awake();
		container = GetComponent<Container>();
		if (container == null)
		{
			Destroy(gameObject);
			return;
		}
		slowdown.Add(0, 0.9f);        
		slowdown.Add(1, 0.85f);
		slowdown.Add(2, 0.70f);
		slowdown.Add(3, 0.55f);
		slowdown.Add(4, 0.40f);
		slowdown.Add(5, 0.25f);
		container.onItemStored += () => { this.Slow(container.GetItemCount()); };
		container.onItemRemoved += () => { this.Slow(container.GetItemCount()); };
		this.Slow(container.GetItemCount());
	}

	internal override void Start()
	{
		base.Start();
		sweat = Instantiate(Game.i.library.sweatParticle, transform).GetComponent<ParticleSystem>();
		sweat.Stop();
		locomotion.locomotionAnimation.legs.onStep += () =>
		{ 
			if(this.itemCount > 0 && this.sweat != null) 
				this.sweat.Play(); 
		};
	}

	public void Slow(int itemStored)
	{

		if(slowdown.TryGetValue(itemStored, out float slow))
		{
			if (this.currentSlow != -1) this.locomotion.RemoveModifier(this.currentSlow);
			currentSlow = locomotion.AddModifier(slow);

		
			if(itemStored > 0) locomotion.locomotionAnimation.HeavyWalkCycle();
			else locomotion.locomotionAnimation.DefaultWalkCycle();

            if (sweat != null) {
                var em = sweat.emission;
                em.rateOverTime = 100f * slow * 10f;
            }
		}
	}
}
