using UnityEngine;

namespace Behavior.NPC
{
    [CreateAssetMenu(menuName = "Behavior/Action/NonPlayableCharacter/ChaseSubject")]
    public class ChaseSubject : AIAction
    {
		public NonPlayableCharacter.ESubject subject;
		public override void Execute(NonPlayableCharacter target)
        {
            NonPlayableCharacter npc = target;
            if(npc != null)
			{
				switch (subject)
				{
					case NonPlayableCharacter.ESubject.PLAYER:
						if (npc.player != null) npc.movement.Chase(npc.player.transform);
						break;
					case NonPlayableCharacter.ESubject.VALUABLE:
						if (npc.valuable != null) npc.movement.Chase(npc.valuable.transform);
						break;
					case NonPlayableCharacter.ESubject.ACTIVITY:
						if (npc.activity != null) npc.movement.Chase(npc.activity.transform);
						break;
					case NonPlayableCharacter.ESubject.CHAIR:
						if (npc.chair != null) npc.movement.Chase(npc.chair.transform);
						break;
					case NonPlayableCharacter.ESubject.FOOD:
						if (npc.food != null) npc.movement.Chase(npc.food.transform);
						break;
					case NonPlayableCharacter.ESubject.FEEDER:
						if (npc.feeder != null) npc.movement.Chase(npc.feeder.transform);
						break;
					case NonPlayableCharacter.ESubject.CHARACTER:
						if (npc.character != null) npc.movement.Chase(npc.character.transform);
						break;
				}
			}
        }
    }
}
