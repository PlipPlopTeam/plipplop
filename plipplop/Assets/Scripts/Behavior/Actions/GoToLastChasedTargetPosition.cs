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
				npc.movement.GoThere(npc.movement.chaseTargetPosition);
				npc.movement.StopChase();
			}
        }
    }
}
