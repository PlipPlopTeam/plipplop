using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Behavior.NPC
{
	[CreateAssetMenu(menuName = "Behavior/Action/NonPlayableCharacter/SearchValuable")]
	public class SearchValuable : AIAction
	{
		public override void Execute(StateManager state)
		{
			NonPlayableCharacter npc = (NonPlayableCharacter)state;
			if(npc != null)
			{
				Valuable[] items = npc.sight.Scan<Valuable>();
				if(items.Length > 0)
				{
					foreach(Valuable item in items)
					{
						if(item.IsVisible()) 
						{
							npc.valuable = item;
							break;
						}
					}
				}
			}
		}
	}
}

