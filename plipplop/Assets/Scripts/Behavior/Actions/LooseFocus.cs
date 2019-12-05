using UnityEngine;
namespace Behavior.NPC
{
	[CreateAssetMenu(menuName = "Behavior/Action/NonPlayableCharacter/LooseFocus")]
	public class LooseFocus : AIAction
	{
		public NonPlayableCharacter.ESubject subject;
		public override void Execute(NonPlayableCharacter target)
		{
			NonPlayableCharacter npc = target;
			if (npc == null) return;
			npc.look.LooseFocus();

		}
	}
}
