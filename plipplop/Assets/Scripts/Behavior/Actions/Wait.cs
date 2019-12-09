using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Behavior.NPC
{
    [CreateAssetMenu(menuName = "Behavior/Action/NonPlayableCharacter/Wait")]
    public class Wait : AIAction
    {
        public float time = 1f;
        public override void Execute(NonPlayableCharacter target)
        {
            NonPlayableCharacter npc = target;
            if (npc != null) npc.Wait(time);
        }
    } 
}

