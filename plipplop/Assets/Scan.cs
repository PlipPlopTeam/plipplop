using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PP;
using UnityEngine.AI;

[CreateAssetMenu(menuName = "Behavior/Action/Scan")]
public class Scan : StateActions
{
	public Sight.Settings sightSettings;
	[HideInInspector] public bool found;
	Sight sight;

	public override void Execute(StateManager states)
	{
		// Adding sight
		if(sight == null)
		{
			sight = states.transform.gameObject.AddComponent<Sight>();
			sight.settings = sightSettings;
		}

		if (sight.Scan<Valuable>().Length > 0)
		{
			Debug.Log("Found");
			found = true;

			if ((Guard)states != null)
			{
				Guard g = (Guard)states;
				g.found = true;
			}
		}
	}
}
