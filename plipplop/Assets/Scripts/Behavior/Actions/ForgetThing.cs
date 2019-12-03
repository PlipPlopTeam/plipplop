using UnityEngine;

namespace Behavior.NPC
{
	[CreateAssetMenu(menuName = "Behavior/Action/NonPlayableCharacter/ForgetThing")]
	public class ForgetThing : AIAction
    {
		public NonPlayableCharacter.EThing thing;
		public override void Execute(NonPlayableCharacter target)
        {
			NonPlayableCharacter npc = target;
			if (npc == null) return;
			switch (thing)
			{
				case NonPlayableCharacter.EThing.PLAYER: npc.player = null; break;
				case NonPlayableCharacter.EThing.VALUABLE: npc.valuable = null; break;
				case NonPlayableCharacter.EThing.ACTIVITY: npc.activity = null; break;
				case NonPlayableCharacter.EThing.CHAIR: npc.chair = null; break;
				case NonPlayableCharacter.EThing.FOOD: npc.food = null; break;
				case NonPlayableCharacter.EThing.FEEDER: npc.feeder = null; break;
			}
		}
	}
}
