using UnityEngine;
using PP;

namespace NPC
{
	[CreateAssetMenu(menuName = "Behavior/Action/NonPlayableCharacter/CarryValuable")]
	public class CarryValuable : StateActions
	{
		public override void Execute(StateManager state)
		{
			NonPlayableCharacter npc = (NonPlayableCharacter)state;
			if(npc != null)
			{
				Controller c = npc.thing.gameObject.GetComponent<Controller>();
                if(c != null)
                {
                    if(Game.i.player.IsPossessing(c))
                        Game.i.player.PossessBaseController();
                }
				npc.agentMovement.ApplyWeightToSpeed(npc.thing.weight, npc.strength);
                npc.thing.transform.position = (npc.skeleton.rightHandBone.position + npc.skeleton.leftHandBone.position)/2f;
                npc.thing.transform.forward = npc.transform.forward;
			}
		}
	}
}
