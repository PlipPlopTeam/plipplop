using UnityEngine;

namespace Behavior.NPC
{
    [CreateAssetMenu(menuName = "Behavior/Action/NonPlayableCharacter/Set Sight Settings")]
    public class SetSightSettings : AIAction
    {
        public Sight.Settings settings;

        public override void Execute(NonPlayableCharacter target)
        {
            NonPlayableCharacter npc = target;
            if (npc != null && npc.sight != null)
			{
				npc.sight.settings = this.settings;
            }
        }
    }
}
