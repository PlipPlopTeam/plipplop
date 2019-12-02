using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Behavior.NPC
{
    [CreateAssetMenu(menuName = "Behavior/Action/NonPlayableCharacter/Set Animator Boolean")]
    public class SetAnimatorBool : AIAction
    {
        public string boolName;
        public bool boolValue;

        public override void Execute(NonPlayableCharacter target)
        {
            NonPlayableCharacter npc = target;
            if (npc != null)
			{
                npc.animator.SetBool(boolName, boolValue);
			}
        }
    }
}
