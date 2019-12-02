using UnityEngine;

namespace Behavior.NPC
{
	[CreateAssetMenu(menuName = "Behavior/Action/NonPlayableCharacter/LeaveActivity")]
	public class LeaveActivity : AIAction
    {
		public override void Execute(NonPlayableCharacter target)
        {
            NonPlayableCharacter npc = target;
            if (npc != null && npc.activity != null)
			{
				npc.activity.Exit(npc);
			}
		}
	}
}
