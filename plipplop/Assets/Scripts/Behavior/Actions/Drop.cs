using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PP;

namespace NPC
{
    [CreateAssetMenu(menuName = "Behavior/Action/NonPlayableCharacter/Drop")]
    public class Drop : StateActions
    {
        public override void Execute(StateManager state)
        {
            NonPlayableCharacter npc = (NonPlayableCharacter)state;
			if(npc != null && npc.thing != null)
			{
				npc.thing.transform.position = (npc.skeleton.rightHandBone.position + npc.skeleton.leftHandBone.position)/2f + npc.transform.forward;
                npc.animator.SetBool("Carrying", false);
            }
        }
    } 
}

