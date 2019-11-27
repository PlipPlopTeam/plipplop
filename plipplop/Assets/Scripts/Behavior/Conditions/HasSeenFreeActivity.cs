using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Behavior.NPC
{
    
    [CreateAssetMenu(menuName = "Behavior/Condition/NonPlayableCharacter/HasSeenFreeActivity")]
	public class HasSeenFreeActivity : Condition
	{
		public override bool Check(StateManager state)
		{
			NonPlayableCharacter npc = (NonPlayableCharacter)state;
			return npc != null && npc.activity != null;
		}
	}	
}

