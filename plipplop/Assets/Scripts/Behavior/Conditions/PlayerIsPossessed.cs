using UnityEngine;
namespace Behavior.NPC
{
	[CreateAssetMenu(menuName = "Behavior/Condition/NonPlayableCharacter/PlayerIsPossessed")]
	public class PlayerIsPossessed : Condition
	{
		public override bool Check(AIState state, NonPlayableCharacter target)
		{
			NonPlayableCharacter npc = target;
			if(npc != null && npc.player != null)
			{
				Controller c = npc.player.GetComponent<Controller>();
				if(c != null && c.IsPossessed())
				{
					return true;
				}
			}
			return false;
		}
	}	
}
