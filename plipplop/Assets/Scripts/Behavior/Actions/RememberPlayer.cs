using UnityEngine;
namespace Behavior.NPC
{
	[CreateAssetMenu(menuName = "Behavior/Action/NonPlayableCharacter/RememberPlayer")]
	public class RememberPlayer : AIAction
	{
		public float time = 5f;
		public override void Execute(NonPlayableCharacter target)
		{
			NonPlayableCharacter npc = target;
			if (npc != null) npc.RememberPlayerFor(time);
		}
	}
}

