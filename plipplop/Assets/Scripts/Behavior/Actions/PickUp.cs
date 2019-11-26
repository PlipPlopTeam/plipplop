using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Behavior.NPC
{
    [CreateAssetMenu(menuName = "Behavior/Action/NonPlayableCharacter/PickUp")]
    public class PickUp : AIAction
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
                npc.valuable.transform.position = npc.skeleton.GetCenterOfHands();
                npc.valuable.transform.forward = npc.transform.forward;
			}
        }
    }
}
