using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Behavior.NPC
{
    [CreateAssetMenu(menuName = "Behavior/Action/NonPlayableCharacter/ShowEmotion")]
    public class ShowEmotion : AIAction
    {
        public Emotion.EVerb verb;
        public string[] subjects;
        public Emotion.EBubble bubbleType;
        public float duration = 1f;

        public override void Execute(NonPlayableCharacter target)
        {
            NonPlayableCharacter npc = target;
            if (npc != null)
			{
				npc.emo.Show(verb, subjects, bubbleType);
            }
        }
    }
}
