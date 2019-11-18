using UnityEngine;
using PP;

namespace NPC
{
	[CreateAssetMenu(menuName = "Behavior/Action/NonPlayableCharacter/LeaveActivity")]
	public class LeaveActivity : Action
	{
		public override void Execute(StateManager state)
		{
			NonPlayableCharacter npc = (NonPlayableCharacter)state;
			if(npc != null && npc.activity != null)
			{
				npc.activity.Exit(npc);
			}
		}
	}
}
