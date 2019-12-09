using UnityEngine;

namespace Behavior.NPC
{
	[CreateAssetMenu(menuName = "Behavior/Action/NonPlayableCharacter/ForgetThing")]
	public class ForgetThing : AIAction
    {
		public NonPlayableCharacter.ESubject thing;
		public override void Execute(NonPlayableCharacter target)
        {
			NonPlayableCharacter npc = target;
			if (npc == null) return;
			switch (thing)
			{
				case NonPlayableCharacter.ESubject.PLAYER: npc.player = null; break;
				case NonPlayableCharacter.ESubject.VALUABLE: npc.valuable = null; break;
				case NonPlayableCharacter.ESubject.ACTIVITY: npc.activity = null; break;
				case NonPlayableCharacter.ESubject.CHAIR: npc.chair = null; break;
				case NonPlayableCharacter.ESubject.FOOD: npc.food = null; break;
				case NonPlayableCharacter.ESubject.FEEDER: npc.feeder = null; break;
			}
		}
	}
}
