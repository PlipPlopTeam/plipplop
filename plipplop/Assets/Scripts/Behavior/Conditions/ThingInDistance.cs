using UnityEngine;

namespace Behavior.NPC
{
	[CreateAssetMenu(menuName = "Behavior/Condition/NonPlayableCharacter/ThingInDistance")]
	public class ThingInDistance : Condition
	{
		[Header("Settings")]
		public NonPlayableCharacter.ESubject thing;
		public Vector2 range;

		public override bool Check(AIState state, NonPlayableCharacter target)
		{
			NonPlayableCharacter npc = target;
			if (npc == null) return false;
			switch (thing)
			{
				case NonPlayableCharacter.ESubject.PLAYER: return npc.player != null && InDistance(npc.transform, npc.player.transform);
				case NonPlayableCharacter.ESubject.VALUABLE: return npc.valuable != null && InDistance(npc.transform, npc.valuable.transform);
				case NonPlayableCharacter.ESubject.ACTIVITY: return npc.activity != null && InDistance(npc.transform, npc.activity.transform);
				case NonPlayableCharacter.ESubject.CHAIR: return npc.chair != null && InDistance(npc.transform, npc.chair.transform);
				case NonPlayableCharacter.ESubject.FOOD: return npc.food != null && InDistance(npc.transform, npc.food.transform);
				case NonPlayableCharacter.ESubject.FEEDER: return npc.feeder != null && InDistance(npc.transform, npc.feeder.transform);
			}
			return false;
		}

		public bool InDistance(Transform tnpc, Transform t)
		{
			float d = Vector3.Distance(t.position, tnpc.position);
			if (d >= range.x && d <= range.y) return true;
			else return false;
		}
	}	
}