using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Behavior.NPC
{
    [CreateAssetMenu(menuName = "Behavior/Action/NonPlayableCharacter/GoToValuableOriginPosition")]
    public class GoToValuableOriginPosition : AIAction
    {
        public override void Execute(NonPlayableCharacter target)
        {
            NonPlayableCharacter npc = target;
            if (npc != null && npc.valuable != null)
			{
				if (npc.movement.GoThere(npc.valuable.origin))
				{
                    npc.movement.onDestinationReached += () => npc.Drop();
				}
                else
                {
                    npc.Drop();
                    npc.movement.reached = true;
                }
			}
        }
    }
}
