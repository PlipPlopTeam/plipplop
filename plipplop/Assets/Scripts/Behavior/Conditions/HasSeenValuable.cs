using UnityEngine;

namespace Behavior.NPC
{
	[CreateAssetMenu(menuName = "Behavior/Condition/NonPlayableCharacter/HasSeenValuable")]
	public class HasSeenValuable : Condition
	{
		public override bool Check(AIState state)
		{
			NonPlayableCharacter npc = state.GetGraphTarget();
			return npc != null && npc.valuable != null;
		}
	}	
}
