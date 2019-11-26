using UnityEngine;
using PP;

namespace NPC
{
	[CreateAssetMenu(menuName = "Behavior/Action/NonPlayableCharacter/ConsumeFood")]
	public class ConsumeFood : AIAction
    {
		public override void Execute(StateManager state)
		{
			NonPlayableCharacter npc = (NonPlayableCharacter)state;
			if(npc != null) npc.Consume(npc.food);
		}
	}
}
