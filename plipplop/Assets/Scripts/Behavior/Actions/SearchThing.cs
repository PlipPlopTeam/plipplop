using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Behavior.NPC {

    [CreateAssetMenu(menuName = "Behavior/Action/NonPlayableCharacter/SearchActivity")]
	public class SearchThing : AIAction
	{
		public NonPlayableCharacter.ESubject thing;
		public override void Execute(NonPlayableCharacter target)
        {
			NonPlayableCharacter npc = target;
			if (npc == null) return;
			switch (thing)
			{
				case NonPlayableCharacter.ESubject.PLAYER:
					Controller[] controllers = npc.sight.Scan<Controller>();
					foreach (Controller c in controllers)
					{
						if (c.IsVisibleByNPC() && c != npc.rPlayer) npc.player = c; break;
					}
					break;
				case NonPlayableCharacter.ESubject.VALUABLE:
					Valuable[] valuables = npc.sight.Scan<Valuable>();
					foreach (Valuable v in valuables)
					{
						if (!v.IsCarried() && v.IsVisible()) npc.valuable = v; break;
					}
					break;
				case NonPlayableCharacter.ESubject.ACTIVITY:
					Activity[] activities = npc.sight.Scan<Activity>();
					foreach (Activity a in activities)
					{
						if(a.AvailableFor(npc))
						{
							if(npc.settings.doAllActivities)
							{
								npc.activity = a;
								break;
							}
							else
							{
								if(npc.settings.favoriteActivities.Contains(a.GetType().ToString()))
								{
									npc.activity = a;
									break;
								}
							}
						}
					}
					break;
				case NonPlayableCharacter.ESubject.CHAIR:
					Chair[] chairs = npc.sight.Scan<Chair>();
					foreach (Chair c in chairs)
					{
						if (!c.IsFull()) npc.chair = c; break;
					}
					break;
				case NonPlayableCharacter.ESubject.FOOD:
					Food[] foods = npc.sight.Scan<Food>();
					foreach (Food f in foods)
					{
						if (!f.consumed && !f.isBeingConsumed) npc.food = f; break;
					}
					break;
				case NonPlayableCharacter.ESubject.FEEDER:
					Feeder[] feeders = npc.sight.Scan<Feeder>();
					foreach (Feeder f in feeders)
					{
						if (!f.IsEmpty()) npc.feeder = f; break;
					}
					break;
			}
		}
	}
}

