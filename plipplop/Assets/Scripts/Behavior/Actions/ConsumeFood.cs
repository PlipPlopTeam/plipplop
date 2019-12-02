using UnityEngine;


namespace Behavior.NPC
{
	[CreateAssetMenu(menuName = "Behavior/Action/NonPlayableCharacter/ConsumeFood")]
	public class ConsumeFood : AIAction
    {
		public override void Execute(NonPlayableCharacter target)
        {
            NonPlayableCharacter npc = target;
            if (npc != null) npc.Consume(npc.food);
		}
	}
}
