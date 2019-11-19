using UnityEngine;
using PP;

namespace NPC
{
	[CreateAssetMenu(menuName = "Behavior/Condition/NonPlayableCharacter/HasSeenFeeder")]
	public class HasSeenFeeder : Condition
	{
		public override bool Check(StateManager state)
		{
			NonPlayableCharacter npc = (NonPlayableCharacter)state;
			return npc != null && npc.feeder != null;
		}
	}	
}