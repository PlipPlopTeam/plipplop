using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Behavior.NPC
{
	[CreateAssetMenu(menuName = "Behavior/Condition/NonPlayableCharacter/ThingInRange")]
	public class ThingInRange : Condition
	{
		public NonPlayableCharacter.EThing thing;
		public override bool Check(AIState state, NonPlayableCharacter target)
		{
			NonPlayableCharacter npc = target;
			if (npc == null) return false;
			switch (thing)
			{
				case NonPlayableCharacter.EThing.PLAYER: return npc.range.IsInRange(npc.player.gameObject);
				case NonPlayableCharacter.EThing.VALUABLE: return npc.range.IsInRange(npc.valuable.gameObject);
				case NonPlayableCharacter.EThing.ACTIVITY: return npc.range.IsInRange(npc.activity.gameObject);
				case NonPlayableCharacter.EThing.CHAIR: return npc.range.IsInRange(npc.chair.gameObject);
				case NonPlayableCharacter.EThing.FOOD: return npc.range.IsInRange(npc.food.gameObject);
				case NonPlayableCharacter.EThing.FEEDER: return npc.range.IsInRange(npc.feeder.gameObject);
			}
			return false;
			}
	}	
}
