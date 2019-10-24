﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PP;

namespace NPC
{
    [CreateAssetMenu(menuName = "Behavior/Action/NonPlayableCharacter/GoToValuableOriginPosition")]
    public class GoToValuableOriginPosition : StateActions
    {
        public override void Execute(StateManager state)
        {
            NonPlayableCharacter npc = (NonPlayableCharacter)state;
			if(npc != null && npc.thing != null)
			{
                npc.agentMovement.GoThere(npc.thing.origin);
			}
        }
    }
}
