using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PP;

namespace NPC
{
	[CreateAssetMenu(menuName = "Behavior/Action/NonPlayableCharacter/GoToNextPathPoint")]
	public class GoToNextPoint : StateActions
	{
		public bool overrideMovement = false;
		public AgentMovement.Settings overrideMovementSettings;
		public override void Execute(StateManager state)
		{
			NonPlayableCharacter npc = (NonPlayableCharacter)state;
			if(npc != null)
			{
				if(overrideMovement) npc.agentMovement.settings = overrideMovementSettings;
				npc.agentMovement.GoToNextPoint();
			}
		}
	}
}
