using UnityEngine;
using PP;

namespace NPC
{
	[CreateAssetMenu(menuName = "Behavior/Action/NonPlayableCharacter/EnterActivity")]
	public class EnterActivity : Action
	{
		public override void Execute(StateManager state)
		{
			NonPlayableCharacter npc = (NonPlayableCharacter)state;
			if(npc != null && npc.activity != null)
			{
				npc.activity.Enter(npc);
			}
		}
	}
}
