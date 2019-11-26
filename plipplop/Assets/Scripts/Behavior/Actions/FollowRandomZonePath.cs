using UnityEngine;
using PP;

namespace NPC
{
	[CreateAssetMenu(menuName = "Behavior/Action/NonPlayableCharacter/FollowRandomZonePath")]
	public class FollowRandomZonePath : Action
	{
		public override void Execute(StateManager state)
		{
			NonPlayableCharacter npc = (NonPlayableCharacter)state;
			if(npc != null)
			{
				npc.agentMovement.ClearEvents();
                npc.agentMovement.FollowPath(Game.i.aiZone.GetRandomPath());
			}
		}
	}
}
