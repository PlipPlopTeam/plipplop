using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Behavior.NPC
{
    [CreateAssetMenu(menuName = "Behavior/Action/NonPlayableCharacter/Drop")]
    public class Drop : AIAction
    {
        public override void Execute(NonPlayableCharacter target)
        {
            NonPlayableCharacter npc = target;
            if (npc != null && npc.valuable != null)
			{
				npc.valuable.transform.position = npc.skeleton.GetCenterOfHands();
                npc.agentMovement.ResetSpeed();
                npc.animator.SetBool("Carrying", false);
            }
        }
    } 
}

