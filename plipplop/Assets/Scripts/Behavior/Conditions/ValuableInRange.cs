using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Behavior.NPC
{
	[CreateAssetMenu(menuName = "Behavior/Condition/NonPlayableCharacter/ValuableInRange")]
	public class ValuableInRange : Condition
	{
		public override bool Check(AIState state, NonPlayableCharacter target)
		{
			NonPlayableCharacter npc = target;
            return npc != null && npc.range.IsInRange(npc.valuable.gameObject);
		}
	}	
}
