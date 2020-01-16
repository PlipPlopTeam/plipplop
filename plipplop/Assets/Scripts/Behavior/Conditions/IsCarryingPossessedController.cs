using UnityEngine;

namespace Behavior.NPC
{
	[CreateAssetMenu(menuName = "Behavior/Condition/NonPlayableCharacter/IsCarryingPossessedController")]
	public class IsCarryingPossessedController : Condition
	{
		public override bool Check(AIState state, NonPlayableCharacter target)
		{
			NonPlayableCharacter npc = target;
			if (npc == null && !npc.IsCarrying()) return false;

			if (npc.carried.Self().gameObject.TryGetComponent(out Controller result))
			{
				if (result.IsPossessed()) return true;
				else return false;
			}
			else return false;
		}
	}
}
