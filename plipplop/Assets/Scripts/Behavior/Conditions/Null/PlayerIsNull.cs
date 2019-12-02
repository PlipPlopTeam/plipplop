using UnityEngine;

namespace Behavior.NPC
{
	[CreateAssetMenu(menuName = "Behavior/Condition/NonPlayableCharacter/PlayerIsNull")]
	public class PlayerIsNull : Condition
	{
		public override bool Check(AIState state, NonPlayableCharacter target)
		{
			NonPlayableCharacter npc = target;
			return npc != null && npc.player == null;
		}
	}	
}