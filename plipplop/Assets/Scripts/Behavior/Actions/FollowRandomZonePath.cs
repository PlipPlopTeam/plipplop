using UnityEngine;

namespace Behavior.NPC
{
	[CreateAssetMenu(menuName = "Behavior/Action/NonPlayableCharacter/FollowRandomZonePath")]
	public class FollowRandomZonePath : AIAction
	{
		public override void Execute(NonPlayableCharacter target)
        {
            NonPlayableCharacter npc = target;
            var randomPath = Game.i.aiZone.GetRandomPath();
            if (npc != null && randomPath)
			{
				npc.agentMovement.ClearEvents();
                npc.agentMovement.FollowPath(randomPath);
			}
		}
	}
}
