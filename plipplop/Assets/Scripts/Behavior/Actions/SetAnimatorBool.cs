using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PP;

namespace NPC
{
    [CreateAssetMenu(menuName = "Behavior/Action/NonPlayableCharacter/Set Animator Boolean")]
    public class SetAnimatorBool : StateActions
    {
        public string boolName;
        public bool boolValue;

        public override void Execute(StateManager state)
        {
            NonPlayableCharacter npc = (NonPlayableCharacter)state;
			if(npc != null)
			{
                npc.animator.SetBool(boolName, boolValue);
			}
        }
    }
}
