using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PP;

namespace NPC
{
    [CreateAssetMenu(menuName = "Behavior/Action/NonPlayableCharacter/ChaseValuable")]
    public class ChaseValuable : StateActions
    {
        public override void Execute(StateManager state)
        {
            NonPlayableCharacter npc = (NonPlayableCharacter)state;
			if(npc != null && npc.thing != null)
			{
                npc.agentMovement.Chase(npc.thing.transform);
			}
        }
    }
}
