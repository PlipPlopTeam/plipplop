﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PP;

namespace NPC
{
	[CreateAssetMenu(menuName = "Behavior/Condition/NonPlayableCharacter/FoodIsNull")]
	public class FoodIsNull : Condition
	{
		public override bool Check(StateManager state)
		{
			NonPlayableCharacter npc = (NonPlayableCharacter)state;
			return npc != null && npc.food == null;
		}
	}	
}