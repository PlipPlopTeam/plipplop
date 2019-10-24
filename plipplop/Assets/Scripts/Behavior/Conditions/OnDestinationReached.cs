using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PP;

namespace NPC
{
    [CreateAssetMenu(menuName = "Behavior/Condition/NonPlayableCharacter/OnDestinationReached")]
    public class OnDestinationReached : Condition
    {
		public override bool Check(StateManager state)
		{
			NonPlayableCharacter npc = (NonPlayableCharacter)state;
			if(npc != null)
			{
				return npc.agentMovement.reached == true;
			}
			return false;
		}
    }  
}
