using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PP;

namespace NPC
{
	[CreateAssetMenu(menuName = "Behavior/Action/NonPlayableCharacter/ForgetThing")]
	public class ForgetThing : Action
	{
		public override void Execute(StateManager state)
		{
			NonPlayableCharacter npc = (NonPlayableCharacter)state;
			if(npc != null)
			{
				npc.valuable = null;
			}
		}
	}
}
