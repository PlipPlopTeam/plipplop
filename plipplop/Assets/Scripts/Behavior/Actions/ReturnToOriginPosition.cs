using UnityEngine;
namespace Behavior.NPC
{
    [CreateAssetMenu(menuName = "Behavior/Action/NonPlayableCharacter/ReturnToOriginPosition")]
    public class ReturnToOriginPosition : AIAction
    {
        public override void Execute(NonPlayableCharacter target)
        {
            NonPlayableCharacter npc = target;
			if(npc != null) npc.agentMovement.GoThere(npc.spawnPosition);
        }
    }
}
