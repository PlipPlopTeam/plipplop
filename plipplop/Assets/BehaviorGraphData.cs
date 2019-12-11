using UnityEngine;
using System.Collections.Generic;

namespace Behavior.Editor
{
	[System.Serializable]
	public class BehaviorGraphData : ScriptableObject
	{
		[System.Serializable]
		public class StateData
		{
			public int id;
			public int stateId;
			public int[] exit;
		}

		[System.Serializable]
		public class TransitionData
		{
			public int id;
			public int conditionId;
			public int[] exit;
		}

		public List<StateData> states = new List<StateData>();
		public List<TransitionData> transitions = new List<TransitionData>();
		public int initialStateId = 0;
		AIStateNode state = null;
		NonPlayableCharacter target = null;

		private int counter = 0;

		public void Compile(BehaviorGraph g)
		{
			NpcLibrary lib = (NpcLibrary)Resources.Load("Assets/Resources/Library");
			Debug.Log(lib);
			counter = 0;
			states.Clear();
			transitions.Clear();

			foreach (AIStateNode sn in g.stateNodes)
			{
				StateData sd = new StateData();
				sd.id = counter++;
				sd.stateId = lib.GetAIStateId(sn.state);
				states.Add(sd);
			}
		}
	}
}
