using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Behavior.NPC
{
	[CreateAssetMenu(menuName = "Behavior/Action/NonPlayableCharacter/SearchFood")]
	public class SearchFood : AIAction
	{
		public override void Execute(NonPlayableCharacter target)
        {
            NonPlayableCharacter npc = target;
            if (npc != null)
			{
				Food[] foods = npc.sight.Scan<Food>();
				if(foods.Length > 0)
				{
                    foreach(Food f in foods)
                    {
                        if(!f.consumed && !f.carried) npc.food = f;
                        break;
                    }
				}
			}
		}
	}
}
