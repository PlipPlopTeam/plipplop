using UnityEngine;

namespace Behavior.NPC
{
	[CreateAssetMenu(menuName = "Behavior/Action/NonPlayableCharacter/ShowOff")]
	public class ShowOff : AIAction
	{
		public float time = 5f;
		public Vector2 range = Vector2.one;
		public int slot = 10;

		public override void Execute(NonPlayableCharacter target)
		{
			NonPlayableCharacter npc = target;
			if (npc != null)
			{
				npc.ShowOff(time, range, slot);
			}
		}
	}
}

