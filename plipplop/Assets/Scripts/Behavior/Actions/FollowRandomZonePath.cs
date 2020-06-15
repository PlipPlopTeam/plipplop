using UnityEngine;

namespace Behavior.NPC
{
	[CreateAssetMenu(menuName = "Behavior/Action/NonPlayableCharacter/FollowRandomZonePath")]
	public class FollowRandomZonePath : AIAction
	{
		public override void Execute(NonPlayableCharacter target)
        {
			if (Game.i.aiZone.GetPaths().Length == 0) return;
            NonPlayableCharacter npc = target;
            if (npc != null)
			{
				if(npc.settings.followPath)
				{
					if(npc.assignedPath != null)
					{
						var randomPath = Game.i.aiZone.GetRandomPath();
						if (randomPath != null) npc.movement.FollowPath(randomPath);
					}
					else
					{
						npc.movement.FollowPath(npc.assignedPath);
					}
				}
				else
				{
					npc.movement.GoThere(npc.spawnPosition);
				}
			}
		}
	}
}
