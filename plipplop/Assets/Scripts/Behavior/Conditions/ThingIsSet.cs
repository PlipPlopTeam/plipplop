using UnityEngine;

namespace Behavior.NPC
{
	[CreateAssetMenu(menuName = "Behavior/Condition/NonPlayableCharacter/ThingIsSet")]
	public class ThingIsSet : Condition
	{
		public NonPlayableCharacter.ESubject subject;
		public override bool Check(AIState state, NonPlayableCharacter target)
		{
			NonPlayableCharacter npc = target;
			if (npc == null) return false;
			switch(subject)
			{
				case NonPlayableCharacter.ESubject.PLAYER: return npc.player != null;
				case NonPlayableCharacter.ESubject.VALUABLE: return npc.valuable != null;
				case NonPlayableCharacter.ESubject.ACTIVITY: return npc.activity != null;
				case NonPlayableCharacter.ESubject.CHAIR: return npc.chair != null;
				case NonPlayableCharacter.ESubject.FOOD: return npc.food != null;
				case NonPlayableCharacter.ESubject.FEEDER: return npc.feeder != null;
			}
			return false;
		}
	}	
}