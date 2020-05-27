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
				return npc.movement.reached == true;
			}
			return false;
		}
    }  
}
