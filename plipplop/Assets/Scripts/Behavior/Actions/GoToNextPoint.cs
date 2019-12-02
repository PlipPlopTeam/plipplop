using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Behavior.NPC
{
	[CreateAssetMenu(menuName = "Behavior/Action/NonPlayableCharacter/GoToNextPathPoint")]
	public class GoToNextPoint : AIAction
    {
		public bool overrideMovement = false;
		public AgentMovement.Settings overrideMovementSettings;
		public override void Execute(NonPlayableCharacter target)
        {
            NonPlayableCharacter npc = target;
            if (npc != null)
			{
				if(overrideMovement) npc.agentMovement.settings = overrideMovementSettings;
				npc.agentMovement.GoToNextPoint();
			}
		}
	}
}
