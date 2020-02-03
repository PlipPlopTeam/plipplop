using UnityEngine;
namespace Behavior.NPC
{
	[CreateAssetMenu(menuName = "Behavior/Condition/NonPlayableCharacter/PlayerIsBaseController")]
	public class PlayerIsBaseController : Condition
	{
		public override bool Check(AIState state, NonPlayableCharacter target)
		{
			NonPlayableCharacter npc = target;
			return npc != null && npc.player != null && npc.player.gameObject.GetComponent<BaseController>() != null;
		}
	}	
}