using UnityEngine;

namespace Behavior.NPC
{
	[CreateAssetMenu(menuName = "Behavior/Action/NonPlayableCharacter/GoAroundInRange")]
	public class GoAroundInRange : AIAction
    {
		public float range;
		public float minRange;
		public float maxRange;

		public override void Execute(NonPlayableCharacter target)
        {
            NonPlayableCharacter npc = target;
            if (npc != null)
			{
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
