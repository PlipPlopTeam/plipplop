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
				Vector3 pos = new Vector3(npc.player.transform.position.x, npc.transform.position.y, npc.player.transform.position.z);



				Vector3 direction = (npc.transform.position - pos).normalized;
				Vector3 position = npc.transform.position + direction * Random.Range(distanceRange.x, distanceRange.y);
				npc.movement.GoThere(position);
				npc.emo.Show(Emotion.EVerb.SURPRISE, "plipplop");
			}

		}
	}
}
