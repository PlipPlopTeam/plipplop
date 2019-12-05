using UnityEngine;
namespace Behavior.NPC
{
	[CreateAssetMenu(menuName = "Behavior/Condition/NonPlayableCharacter/SubjectDistanceCheck")]
	public class SubjectDistanceCheck : Condition
	{
		[Header("Settings")]
		public NonPlayableCharacter.ESubject subject;
		public StatCheck.EOperator condition;
		public float value = 0f;

		public override bool Check(AIState state, NonPlayableCharacter target)
		{
			NonPlayableCharacter npc = target;
			if (npc == null) return false;
			switch (subject)
			{
				case NonPlayableCharacter.ESubject.PLAYER:
					if (npc.player == null) return false;
					return Valid(Vector3.Distance(npc.transform.position, npc.player.transform.position), value);
				case NonPlayableCharacter.ESubject.VALUABLE:
					if (npc.valuable == null) return false;
					return Valid(Vector3.Distance(npc.transform.position, npc.valuable.transform.position), value);
				case NonPlayableCharacter.ESubject.ACTIVITY:
					if (npc.activity == null) return false;
					return Valid(Vector3.Distance(npc.transform.position, npc.activity.transform.position), value);
				case NonPlayableCharacter.ESubject.CHAIR:
					if (npc.chair == null) return false;
					return Valid(Vector3.Distance(npc.transform.position, npc.chair.transform.position), value);
				case NonPlayableCharacter.ESubject.FOOD:
					if (npc.food == null) return false;
					return Valid(Vector3.Distance(npc.transform.position, npc.food.transform.position), value);
				case NonPlayableCharacter.ESubject.FEEDER:
					if (npc.feeder == null) return false;
					return Valid(Vector3.Distance(npc.transform.position, npc.feeder.transform.position), value);
			}
			return false;
		}

		public bool Valid(float distance, float value)
		{
			switch (condition)
			{
				case StatCheck.EOperator.EQUAL:
					if (distance == value) return true;
					break;
				case StatCheck.EOperator.SUPERIOR:
					if (distance > value) return true;
					break;
				case StatCheck.EOperator.SUPERIOR_OR_EQUAL:
					if (distance >= value) return true;
					break;
				case StatCheck.EOperator.INFERIOR:
					if (distance < value) return true;
					break;
				case StatCheck.EOperator.INFERIOR_OR_EQUAL:
					if (distance <= value) return true;
					break;
			}
			return false;
		}
	}
}