using UnityEngine;
using PP;

namespace NPC
{
	[CreateAssetMenu(menuName = "Behavior/Action/NonPlayableCharacter/FollowRandomZonePath")]
	public class FollowRandomZonePath : StateActions
	{
		public override void Execute(StateManager state)
		{
			NonPlayableCharacter npc = (NonPlayableCharacter)state;
			if(npc != null)
			{
				npc.agentMovement.ClearEvents();
                npc.agentMovement.FollowPath(Zone.i.GetRandomPath());
			}
		}
	}
}
