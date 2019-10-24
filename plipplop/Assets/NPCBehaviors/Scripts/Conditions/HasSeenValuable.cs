using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PP;

namespace NPC
{
	[CreateAssetMenu(menuName = "Behavior/Condition/NonPlayableCharacter/HasSeenValuable")]
	public class HasSeenValuable : Condition
	{
		public override bool CheckCondition(StateManager state)
		{
			NonPlayableCharacter npc = (NonPlayableCharacter)state;
			if(npc != null)
			{
				return npc.thing == true;
			}
			return false;
		}
	}	
}

