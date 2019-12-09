using UnityEngine;
namespace Behavior.NPC
{
	[CreateAssetMenu(menuName = "Behavior/Action/NonPlayableCharacter/LookAtSubject")]
	public class LookAtSubject : AIAction
	{
		public NonPlayableCharacter.ESubject subject;
		public override void Execute(NonPlayableCharacter target)
		{
			NonPlayableCharacter npc = target;
			if (npc == null) return;
			switch (subject)
			{
				case NonPlayableCharacter.ESubject.PLAYER:
					if (npc.player != null) npc.look.FocusOn(npc.player.transform);
					break;
				case NonPlayableCharacter.ESubject.VALUABLE:
					if (npc.valuable != null) npc.look.FocusOn(npc.valuable.transform);
					break;
				case NonPlayableCharacter.ESubject.ACTIVITY:
					if (npc.activity != null) npc.look.FocusOn(npc.activity.transform);
					break;
				case NonPlayableCharacter.ESubject.CHAIR:
					if (npc.chair != null) npc.look.FocusOn(npc.chair.transform);
					break;
				case NonPlayableCharacter.ESubject.FOOD:
					if (npc.food != null) npc.look.FocusOn(npc.food.transform);
					break;
				case NonPlayableCharacter.ESubject.FEEDER:
					if (npc.feeder != null) npc.look.FocusOn(npc.feeder.transform);
					break;
			}

		}
	}
}
