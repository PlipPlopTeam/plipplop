using UnityEngine;

namespace Behavior.NPC
{

	[CreateAssetMenu(menuName = "Behavior/Condition/NonPlayableCharacter/IsSubjectVisible")]
	public class IsSubjectVisible : Condition
	{
		public NonPlayableCharacter.ESubject thing;
		public override bool Check(AIState state, NonPlayableCharacter target)
		{
			NonPlayableCharacter npc = target;
			if (npc == null) return false;
			switch (thing)
			{
				case NonPlayableCharacter.ESubject.PLAYER:
					return npc.player != null && npc.sight.See(npc.player.gameObject) && npc.player.IsVisibleByNPC();
				case NonPlayableCharacter.ESubject.VALUABLE:
					return npc.valuable != null && npc.sight.See(npc.valuable.gameObject) && npc.valuable.IsVisible();
				case NonPlayableCharacter.ESubject.ACTIVITY:
					return npc.activity != null && npc.sight.See(npc.activity.gameObject);
				case NonPlayableCharacter.ESubject.CHAIR:
					return npc.chair != null && npc.sight.See(npc.chair.gameObject);
				case NonPlayableCharacter.ESubject.FOOD:
					return npc.food != null && npc.sight.See(npc.food.gameObject);
				case NonPlayableCharacter.ESubject.FEEDER:
					return npc.feeder != null && npc.sight.See(npc.feeder.gameObject);
			}
			return false;
		}
	}
}

