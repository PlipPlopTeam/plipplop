using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PP;

namespace NPC
{
    [CreateAssetMenu(menuName = "Behavior/Action/NonPlayableCharacter/ChaseFood")]
    public class ChaseFood : Action
    {
        public override void Execute(StateManager state)
        {
            NonPlayableCharacter npc = (NonPlayableCharacter)state;
			if(npc != null && npc.food != null)
			{
                npc.Collect(npc.food);
			}
        }
    }
}
