﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Behavior.NPC
{
	[CreateAssetMenu(menuName = "Behavior/Action/NonPlayableCharacter/WaitBetween")]
	public class WaitBetween : AIAction
	{
		public Vector2 range;
		public override void Execute(NonPlayableCharacter target)
		{
			// Prevent Vector2 settings from breaking the Rand.Range
			if (range.x < range.y)
				range.x = range.y - 0.1f;
			else if (range.y < range.x)
				range.y = range.x + 0.1f;

			NonPlayableCharacter npc = target;
			if (npc != null) npc.Wait(Random.Range(range.x, range.y));
		}
	}
}

