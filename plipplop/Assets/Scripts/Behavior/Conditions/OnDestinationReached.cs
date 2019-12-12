using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Behavior.NPC
{
    [CreateAssetMenu(menuName = "Behavior/Condition/NonPlayableCharacter/OnDestinationReached")]
    public class OnDestinationReached : Condition
    {
		public override bool Check(AIState state, NonPlayableCharacter target)
		{
			NonPlayableCharacter npc = target;
			if(npc != null)
			{
				if(npc.agentMovement.reached == true) Debug.Log("=== DESTINATION REACHED ===");
				return npc.agentMovement.reached == true;
			}
			return false;
		}
    }  
}
