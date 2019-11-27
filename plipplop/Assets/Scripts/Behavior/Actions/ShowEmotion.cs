using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Behavior.NPC
{
    [CreateAssetMenu(menuName = "Behavior/Action/NonPlayableCharacter/ShowEmotion")]
    public class ShowEmotion : AIAction
    {
        public Emotion emotion = null;
        public float duration = 1f;

        public override void Execute(StateManager state)
        {
            NonPlayableCharacter npc = (NonPlayableCharacter)state;
			if(npc != null && emotion != null)
			{
				npc.emo.Show(emotion, duration);
            }
        }
    }
}
