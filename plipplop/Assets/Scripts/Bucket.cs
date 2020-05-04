using System.Collections.Generic;
using UnityEngine;

public class Bucket : Controller
{
	Dictionary<int, float> slowdown = new Dictionary<int, float>();
	int currentSlow = -1;
	Container container;
	ParticleSystem sweat;
    public int itemCount { get { return container.GetItemCount(); } }

	public override void OnPossess()
	{
		base.OnPossess();
		locomotion.locomotionAnimation.HeavyWalkCycle();
	}

	public override void OnEject()
	{
		base.OnEject();
		locomotion.locomotionAnimation.DefaultWalkCycle();
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
		locomotion.locomotionAnimation.legs.onStep += () => { this.sweat.Play(); };
	}

	public void Slow(int itemStored)
	{

		if(slowdown.TryGetValue(itemStored, out float slow))
		{
			if (this.currentSlow != -1) this.locomotion.RemoveModifier(this.currentSlow);
			currentSlow = locomotion.AddModifier(slow);
			//sweat.emission.burstCount = Random.Range(6, 10) + Mathf.Round(slow * 10f);
		}
	}
}
