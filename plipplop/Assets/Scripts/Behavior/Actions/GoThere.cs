using UnityEngine;

namespace Behavior.NPC
{
	[CreateAssetMenu(menuName = "Behavior/Action/NonPlayableCharacter/GoThere")]
	public class GoThere : AIAction
	{
		public Vector3 position;
		public override void Execute(NonPlayableCharacter target)
        {
            NonPlayableCharacter npc = target;
            if (npc != null) npc.movement.GoThere(position);
		}
	}
}
