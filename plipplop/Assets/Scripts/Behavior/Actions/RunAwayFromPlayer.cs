using UnityEngine;

namespace Behavior.NPC
{
	[CreateAssetMenu(menuName = "Behavior/Action/NonPlayableCharacter/RunAwayFromPlayer")]
	public class RunAwayFromPlayer : AIAction
    {
		public Vector2 distanceRange;
		public override void Execute(NonPlayableCharacter target)
        {
            NonPlayableCharacter npc = target;
            if (npc != null && npc.player != null)
			{
				Vector3 direction = -(npc.transform.position - npc.player.transform.position).normalized;
				Vector3 position = npc.transform.position + direction * Random.Range(distanceRange.x, distanceRange.y);
				npc.agentMovement.GoThere(position);
			}

		}
	}
}
