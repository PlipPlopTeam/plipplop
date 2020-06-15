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
			if (npc != null && npc.settings.followPath && npc.movement.path == null)
			{
				if(npc.assignedPath != null)
				{
					npc.movement.FollowPath(npc.assignedPath);
				}
				else
				{
					var randomPath = Game.i.aiZone.GetRandomPath();
					npc.movement.FollowPath(randomPath);
				}
			}
		}
	}
}
