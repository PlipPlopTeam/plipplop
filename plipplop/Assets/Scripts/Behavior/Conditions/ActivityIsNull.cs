using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Behavior.NPC
{
	[CreateAssetMenu(menuName = "Behavior/Condition/NonPlayableCharacter/ActivityIsNull")]
	public class ActivityIsNull : Condition
	{
		public override bool Check(AIState state)
		{
			NonPlayableCharacter npc = state.GetGraphTarget();
			return npc != null && npc.activity == null;
		}
	}	
}