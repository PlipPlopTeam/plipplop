using UnityEngine;
using PP;

namespace NPC
{
    [CreateAssetMenu(menuName = "Behavior/Action/NonPlayableCharacter/Set Sight Settings")]
    public class SetSightSettings : AIAction
    {
        public Sight.Settings settings;

        public override void Execute(StateManager state)
        {
            NonPlayableCharacter npc = (NonPlayableCharacter)state;
			if(npc != null && npc.sight != null)
			{
				npc.sight.settings = this.settings;
            }
        }
    }
}
