using UnityEngine;

namespace Behavior.NPC
{
	[CreateAssetMenu(menuName = "Behavior/Action/NonPlayableCharacter/GoAroundInRange")]
	public class GoAroundInRange : AIAction
    {
		public float range;

		public bool overrideMovement;
		public AgentMovement.Settings overrideMovementSettings;

		public override void Execute(NonPlayableCharacter target)
        {
            NonPlayableCharacter npc = target;
            if (npc != null)
			{
				if (overrideMovement) npc.agentMovement.settings = overrideMovementSettings;
				npc.agentMovement.GoThere(
					new Vector3(
						npc.transform.position.x + Random.Range(-range, range),
						npc.transform.position.y, 
						npc.transform.position.z + Random.Range(-range, range)
            		)
				);
			}

		}
	}
}
