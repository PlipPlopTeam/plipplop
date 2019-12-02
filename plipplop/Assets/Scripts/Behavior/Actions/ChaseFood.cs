using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Behavior.NPC
{
    [CreateAssetMenu(menuName = "Behavior/Action/NonPlayableCharacter/ChaseFood")]
    public class ChaseFood : AIAction
    {
        public override void Execute(NonPlayableCharacter target)
        {
            NonPlayableCharacter npc = target;
			if(npc != null && npc.food != null)
			{
                npc.Collect(npc.food);
			}
        }
    }
}
