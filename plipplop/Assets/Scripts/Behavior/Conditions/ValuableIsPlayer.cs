using UnityEngine;
namespace Behavior.NPC
{
	[CreateAssetMenu(menuName = "Behavior/Condition/NonPlayableCharacter/ValuableIsPlayer")]
	public class ValuableIsPlayer : Condition
	{

		public override bool Check(AIState state, NonPlayableCharacter target)
		{
			NonPlayableCharacter npc = target;
			if (npc != null && npc.valuable != null)
			{
				Controller c = npc.valuable.gameObject.GetComponent<Controller>();
				if (c != null 
					&& c.IsPossessed() 
					&& !c.locomotion.AreLegsRetracted()) return true;
			}
			return false;
		}
	}	
}