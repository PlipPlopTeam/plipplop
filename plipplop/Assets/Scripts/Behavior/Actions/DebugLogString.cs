using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Behavior;

namespace Behavior.NPC
{
	[CreateAssetMenu(menuName = "Behavior/Action/Log")]
	public class DebugLogString : AIAction
	{
		public string log = "LOG";
		public override void Execute(NonPlayableCharacter target)
		{
			Debug.Log(Time.time + " : " + log);
		}
	}
}
