﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Behavior.NPC
{
    [CreateAssetMenu(menuName = "Behavior/Action/NonPlayableCharacter/SetFace")]
    public class SetFace : AIAction
    {
        [Header("Settings")]
        public bool speaking;
        public bool eating;
        public bool winking = true;
        public float happiness = 50f;

        public override void Execute(NonPlayableCharacter target)
        {
            NonPlayableCharacter npc = target;
            if (npc != null)
			{
                npc.face.speaking = speaking;
                npc.face.eating = eating;
                npc.face.winking = winking;
                npc.face.happiness = happiness;
			}
        }
    }
}
