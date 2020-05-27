using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Behavior.NPC
{
	[CreateAssetMenu(menuName = "Behavior/Action/NonPlayableCharacter/StopMovement")]
	public class StopMovement : AIAction
	{
		public override void Execute(NonPlayableCharacter target)
        {
            NonPlayableCharacter npc = target;
            if (npc != null)
			{
				npc.movement.Stop();
			}
		}
	}
}
