using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PP;

namespace NPC
{
    [CreateAssetMenu(menuName = "Behavior/Action/NonPlayableCharacter/SetFace")]
    public class SetFace : Action
    {
        [Header("Settings")]
        public bool speaking;
        public bool eating;
        public bool winking = true;

        public override void Execute(StateManager state)
        {
            NonPlayableCharacter npc = (NonPlayableCharacter)state;
			if(npc != null)
			{
                npc.face.Set(speaking, eating, winking);
			}
        }
    }
}
