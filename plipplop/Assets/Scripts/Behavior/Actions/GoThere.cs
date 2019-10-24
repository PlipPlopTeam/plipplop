using UnityEngine;
using PP;

namespace NPC
{
	[CreateAssetMenu(menuName = "Behavior/Action/NonPlayableCharacter/GoThere")]
	public class GoThere : StateActions
	{
		public Vector3 position;
		public override void Execute(StateManager state)
		{
			NonPlayableCharacter npc = (NonPlayableCharacter)state;
			if(npc != null) npc.agentMovement.GoThere(position);
		}
	}
}
