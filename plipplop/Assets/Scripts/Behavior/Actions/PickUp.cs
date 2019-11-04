using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PP;

namespace NPC
{
    [CreateAssetMenu(menuName = "Behavior/Action/NonPlayableCharacter/PickUp")]
    public class PickUp : StateActions
    {
        public override void Execute(StateManager state)
        {
            NonPlayableCharacter npc = (NonPlayableCharacter)state;
			if(npc != null)
			{
				Controller c = npc.valuable.gameObject.GetComponent<Controller>();
                if(c != null)
                {
                    if(Game.i.player.IsPossessing(c))
                        Game.i.player.TeleportBaseControllerAndPossess();
                }
                npc.valuable.transform.position = (npc.skeleton.rightHandBone.position + npc.skeleton.leftHandBone.position)/2f;
                npc.valuable.transform.forward = npc.transform.forward;
			}
        }
    }
}
