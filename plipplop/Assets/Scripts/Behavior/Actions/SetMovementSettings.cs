using UnityEngine;

namespace Behavior.NPC
{
    [CreateAssetMenu(menuName = "Behavior/Action/NonPlayableCharacter/Set Movement Settings")]
    public class SetMovementSettings : AIAction
    {
        public AgentMovement.Settings settings;

        public override void Execute(NonPlayableCharacter target)
        {
            NonPlayableCharacter npc = target;
            if (npc != null && npc.sight != null)
			{
				npc.movement.settings = this.settings;
            }
        }
    }
}
