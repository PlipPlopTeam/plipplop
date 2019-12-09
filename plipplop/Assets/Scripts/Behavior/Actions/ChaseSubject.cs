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
            if (npc != null && npc.valuable != null)
			{
				switch (subject)
				{
					case NonPlayableCharacter.ESubject.PLAYER:
						if (npc.player != null) npc.agentMovement.Chase(npc.player.transform);
						break;
					case NonPlayableCharacter.ESubject.VALUABLE:
						if (npc.valuable != null) npc.agentMovement.Chase(npc.valuable.transform);
						break;
					case NonPlayableCharacter.ESubject.ACTIVITY:
						if (npc.activity != null) npc.agentMovement.Chase(npc.activity.transform);
						break;
					case NonPlayableCharacter.ESubject.CHAIR:
						if (npc.chair != null) npc.agentMovement.Chase(npc.chair.transform);
						break;
					case NonPlayableCharacter.ESubject.FOOD:
						if (npc.food != null) npc.agentMovement.Chase(npc.food.transform);
						break;
					case NonPlayableCharacter.ESubject.FEEDER:
						if (npc.feeder != null) npc.agentMovement.Chase(npc.feeder.transform);
						break;
				}
			}
        }
    }
}
