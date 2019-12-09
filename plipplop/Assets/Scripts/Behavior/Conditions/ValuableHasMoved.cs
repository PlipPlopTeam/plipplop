using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Behavior.NPC
{
	[CreateAssetMenu(menuName = "Behavior/Condition/NonPlayableCharacter/ValuableHasMoved")]
    public class ValuableHasMoved : Condition
    {
		public override bool Check(AIState state, NonPlayableCharacter target)
		{
			NonPlayableCharacter npc = target;
			return npc != null && npc.valuable != null && npc.valuable.HasMoved();
		}
    }
}
