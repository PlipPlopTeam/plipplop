using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Behavior.NPC
{
	[CreateAssetMenu(menuName = "Behavior/Condition/NonPlayableCharacter/PathIsNull")]
	public class PathIsNull : Condition
	{
		public override bool Check(AIState state, NonPlayableCharacter target)
		{
			NonPlayableCharacter npc = target;
			return npc != null && npc.agentMovement.path == null;
		}
	}	
}