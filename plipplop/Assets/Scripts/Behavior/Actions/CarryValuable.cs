using UnityEngine;
using Behavior;

namespace Behavior.NPC
{
	[CreateAssetMenu(menuName = "Behavior/Action/NonPlayableCharacter/CarryValuable")]
	public class CarryValuable : AIAction
    {
		public override void Execute(NonPlayableCharacter target)
        {
            NonPlayableCharacter npc = target;
            if (npc != null)
			{
				Controller c = npc.valuable.gameObject.GetComponent<Controller>();
                if(c != null)
                {
                    if(Game.i.player.IsPossessing(c))
                        Game.i.player.PossessBaseController();
                }
				npc.Carry(npc.valuable);
			}
		}
	}
}
