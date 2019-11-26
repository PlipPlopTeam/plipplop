using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PP;

namespace NPC
{
    [CreateAssetMenu(menuName = "Behavior/Action/NonPlayableCharacter/Drop")]
    public class Drop : AIAction
    {
        public override void Execute(StateManager state)
        {
            NonPlayableCharacter npc = (NonPlayableCharacter)state;
			if(npc != null && npc.valuable != null)
			{
				npc.valuable.transform.position = npc.skeleton.GetCenterOfHands();
                npc.agentMovement.ResetSpeed();
                npc.animator.SetBool("Carrying", false);
            }
        }
    } 
}

