using UnityEngine;
namespace Behavior.NPC
{
    [CreateAssetMenu(menuName = "Behavior/Action/NonPlayableCharacter/GoToLastChasedTargetPosition")]
    public class GoToLastChasedTargetPosition : AIAction
    {
        public override void Execute(NonPlayableCharacter target)
        {
            NonPlayableCharacter npc = target;
			if(npc != null)
			{
				npc.agentMovement.GoThere(npc.agentMovement.chaseTargetPosition);
				npc.agentMovement.StopChase();
			}
        }
    }
}
