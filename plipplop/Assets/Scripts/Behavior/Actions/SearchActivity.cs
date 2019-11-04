using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PP;

namespace NPC
{
	[CreateAssetMenu(menuName = "Behavior/Action/NonPlayableCharacter/SearchActivity")]
	public class SearchActivity : StateActions
	{
		public override void Execute(StateManager state)
		{
			NonPlayableCharacter npc = (NonPlayableCharacter)state;
			if(npc != null)
			{
				Activity[] activities = npc.sight.Scan<Activity>();
				if(activities.Length > 0)
				{
					foreach(Activity a in activities)
					{
						if(!a.full && a.working && !a.users.Contains(npc)) 
						{
							a.Enter(npc);
							break;
						}
					}
				}
			}
		}
	}
}

