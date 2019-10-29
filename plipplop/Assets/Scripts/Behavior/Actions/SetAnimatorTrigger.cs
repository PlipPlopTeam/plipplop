using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PP;

namespace NPC
{
    [CreateAssetMenu(menuName = "Behavior/Action/NonPlayableCharacter/Set Animator Trigger")]
    public class SetAnimatorTrigger : StateActions
    {
        public string triggerName;

        public override void Execute(StateManager state)
        {
            NonPlayableCharacter npc = (NonPlayableCharacter)state;
			if(npc != null)
			{
                npc.animator.SetTrigger(triggerName);
			}
        }
    }
}
