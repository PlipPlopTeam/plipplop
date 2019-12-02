using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Behavior.NPC
{
	[CreateAssetMenu(menuName = "Behavior/Condition/NonPlayableCharacter/HasSeenFood")]
	public class HasSeenFood : Condition
	{
		public override bool Check(AIState state)
		{
			NonPlayableCharacter npc = state.GetGraphTarget();
			return npc != null && npc.food != null;
		}
	}	
}