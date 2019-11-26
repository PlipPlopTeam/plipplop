using UnityEngine;
using PP;

namespace NPC
{

    [CreateAssetMenu(menuName = "Behavior/Condition/NonPlayableCharacter/StatCheck")]
    public class StatCheck : Condition
    {
        public enum EOperator {EQUAL, SUPERIOR, INFERIOR, SUPERIOR_OR_EQUAL, INFERIOR_OR_EQUAL}

        [Header("Settings")]
        public string statName;
        public EOperator condition;
        [Range(0f, 100f)] public float value = 0;

		public override bool Check(StateManager state)
		{
			NonPlayableCharacter npc = (NonPlayableCharacter)state;
            if(npc != null)
            {
                if(npc.stats.ContainsKey(statName))
                {
                    switch(condition)
                    {
                        case EOperator.EQUAL:
                            if(npc.stats[statName] == value) return true;
                            break;
                        case EOperator.SUPERIOR:
                            if(npc.stats[statName] > value) return true;
                            break;
                        case EOperator.SUPERIOR_OR_EQUAL:
                            if(npc.stats[statName] >= value) return true;
                            break;
                        case EOperator.INFERIOR:
                            if(npc.stats[statName] < value) return true;
                            break;
                        case EOperator.INFERIOR_OR_EQUAL:
                            if(npc.stats[statName] <= value) return true;
                            break;
                    }
                }
                else Debug.LogWarning("Issue in " + name + " condition object, '" + statName + "' dosn't exist");
            }
            return false;
		}
    }
}
