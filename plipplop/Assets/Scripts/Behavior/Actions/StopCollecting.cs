using UnityEngine;
using Behavior;

namespace Behavior.NPC
{
	[CreateAssetMenu(menuName = "Behavior/Action/NonPlayableCharacter/StopCollecting")]
	public class StopCollecting : AIAction
    {
		public override void Execute(NonPlayableCharacter target)
        {
            NonPlayableCharacter npc = target;
            if (npc != null) npc.StopCollecting();
		}
	}
}
