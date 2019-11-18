using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PP;

namespace NPC
{
	[CreateAssetMenu(menuName = "Behavior/Action/NonPlayableCharacter/SearchFood")]
	public class SearchFood : Action
	{
		public override void Execute(StateManager state)
		{
			NonPlayableCharacter npc = (NonPlayableCharacter)state;
			if(npc != null)
			{
				Food[] foods = npc.sight.Scan<Food>();
				if(foods.Length > 0)
				{
                    foreach(Food f in foods)
                    {
                        if(!f.consumed) npc.food = f;
                        break;
                    }
				}
			}
		}
	}
}
