using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Behavior.NPC
{
    
    [CreateAssetMenu(menuName = "Behavior/Action/NonPlayableCharacter/Set Animator Trigger")]
    public class SetAnimatorTrigger : AIAction
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
