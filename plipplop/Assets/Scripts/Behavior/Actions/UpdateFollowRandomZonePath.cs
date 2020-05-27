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
			var randomPath = Game.i.aiZone.GetRandomPath();
			if (npc != null 
				&& npc.settings.followPath 
				&& npc.movement.path == null
				&& randomPath != null)
			{
				npc.movement.FollowPath(randomPath);
			}
		}
	}
}
