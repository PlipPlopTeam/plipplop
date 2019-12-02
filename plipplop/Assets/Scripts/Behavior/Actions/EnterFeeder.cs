using UnityEngine;

namespace Behavior.NPC
{
	[CreateAssetMenu(menuName = "Behavior/Action/NonPlayableCharacter/EnterFeeder")]
	public class EnterFeeder : AIAction
    {
		public override void Execute(NonPlayableCharacter target)
        {
            NonPlayableCharacter npc = target;
            if (npc != null && npc.feeder != null)
			{
				npc.feeder.Catch(npc);
			}
		}
	}
}
