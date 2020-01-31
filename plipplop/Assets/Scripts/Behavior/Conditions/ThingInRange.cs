using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Behavior.NPC
{
	[CreateAssetMenu(menuName = "Behavior/Condition/NonPlayableCharacter/ThingInRange")]
	public class ThingInRange : Condition
	{
		public NonPlayableCharacter.ESubject thing;
		public override bool Check(AIState state, NonPlayableCharacter target)
		{
			NonPlayableCharacter npc = target;
			if (npc == null) return false;
			switch (thing)
			{
				case NonPlayableCharacter.ESubject.PLAYER: return npc.player != null && npc.range.IsInRange(npc.player.gameObject);
				case NonPlayableCharacter.ESubject.VALUABLE: return npc.valuable != null && npc.range.IsInRange(npc.valuable.gameObject);
				case NonPlayableCharacter.ESubject.ACTIVITY: return npc.activity != null && npc.range.IsInRange(npc.activity.gameObject);
				case NonPlayableCharacter.ESubject.CHAIR: return npc.chair != null && npc.range.IsInRange(npc.chair.gameObject);
				case NonPlayableCharacter.ESubject.FOOD: return npc.food != null && npc.range.IsInRange(npc.food.gameObject);
				case NonPlayableCharacter.ESubject.FEEDER: return npc.feeder != null && npc.range.IsInRange(npc.feeder.gameObject);
				case NonPlayableCharacter.ESubject.CHARACTER: return npc.character != null && npc.range.IsInRange(npc.character.gameObject);
			}
			return false;
		}
	}	
}
