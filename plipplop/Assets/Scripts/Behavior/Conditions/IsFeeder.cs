using UnityEngine;

namespace Behavior.NPC
{
	[CreateAssetMenu(menuName = "Behavior/Condition/NonPlayableCharacter/IsFeeder")]
	public class IsFeeder : Condition
	{
		public bool set;

		public override bool Check(AIState state)
		{
			NonPlayableCharacter npc = state.GetGraphTarget();
			if(npc != null)
			{
				if(set) return npc.feeder != null;
				else return npc.feeder == null;
			}
			else return false;
		}
	}	
}