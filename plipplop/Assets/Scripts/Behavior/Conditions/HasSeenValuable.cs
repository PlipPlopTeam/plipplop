using UnityEngine;
using PP;

namespace NPC
{
	[CreateAssetMenu(menuName = "Behavior/Condition/NonPlayableCharacter/HasSeenValuable")]
	public class HasSeenValuable : Condition
	{
		public override bool Check(StateManager state)
		{
			NonPlayableCharacter npc = (NonPlayableCharacter)state;
			return npc != null && npc.valuable != null;
		}
	}	
}
