using UnityEngine;

namespace Behavior.NPC
{
	[CreateAssetMenu(menuName = "Behavior/Action/NonPlayableCharacter/UpdateFollowRandomZonePath")]
	public class UpdateFollowRandomZonePath : AIAction
	{
		public override void Execute(NonPlayableCharacter target)
        {
			if (Game.i.aiZone.GetPaths().Length == 0) return;
            NonPlayableCharacter npc = target;

			if (npc != null && npc.agentMovement.path == null)
			{
				npc.agentMovement.ClearEvents();
                npc.agentMovement.FollowPath(Game.i.aiZone.GetRandomPath());
			}
		}
	}
}
