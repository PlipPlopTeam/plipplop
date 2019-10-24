using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PP;

namespace NPC
{
	[CreateAssetMenu(menuName = "Behavior/Condition/NonPlayableCharacter/ValuableHasMoved")]
    public class ValuableHasMoved : Condition
    {
		public override bool Check(StateManager state)
		{
			NonPlayableCharacter npc = (NonPlayableCharacter)state;
			return npc != null && npc.thing != null && npc.thing.HasMoved();
		}
    }
}
