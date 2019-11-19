using UnityEngine;
using PP;

namespace NPC
{
	[CreateAssetMenu(menuName = "Behavior/Action/NonPlayableCharacter/EnterFeeder")]
	public class EnterFeeder : Action
	{
		public override void Execute(StateManager state)
		{
			NonPlayableCharacter npc = (NonPlayableCharacter)state;
			if(npc != null && npc.feeder != null)
			{
				npc.feeder.Enter(npc);
			}
		}
	}
}
