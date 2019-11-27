using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Behavior.NPC
{
    [CreateAssetMenu(menuName = "Behavior/Action/NonPlayableCharacter/ChaseValuable")]
    public class ChaseValuable : AIAction
    {
        public override void Execute(NonPlayableCharacter target)
        {
            NonPlayableCharacter npc = target;
            if (npc != null && npc.valuable != null)
			{
                npc.agentMovement.Chase(npc.valuable.transform);
			}
        }
    }
}
