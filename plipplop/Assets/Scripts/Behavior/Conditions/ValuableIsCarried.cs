using UnityEngine;

namespace Behavior.NPC
{
	[CreateAssetMenu(menuName = "Behavior/Condition/NonPlayableCharacter/ValuableIsCarried")]
	public class ValuableIsCarried : Condition
	{
		public bool yes = true;
		public override bool Check(AIState state, NonPlayableCharacter target)
		{
			NonPlayableCharacter npc = target;
			if (npc == null || npc.valuable == null) return false;
			{
				if(yes) return npc.valuable.IsCarried();
				else return !npc.valuable.IsCarried();
			}
		}
	}	
}