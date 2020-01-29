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
			public string name;
			public int id = -1;
			public int stateId = -1;
			public int[] exits;
			public bool[] hasExits;
		}

		[System.Serializable]
		public class TransitionData
		{
			public int id = -1;
			public bool disable = false;
			public int[] conditions;
			public int[] exits;
			public bool[] hasExits;
		}

		public List<StateData> states = new List<StateData>();
		public List<TransitionData> transitions = new List<TransitionData>();
		public int initialNode = 0;

		// Not serialized
		private NonPlayableCharacter target;
		private int currentNode = -1;

		public bool IsCurrent(int id)
		{
			return currentNode == id;
		}

		public AIState GetState()
		{
			if (currentNode < 0) return null;
			return Game.i.library.npcLibrary.GetAIStateObject(GetStateNode(currentNode).stateId);
		}
		public StateData GetStateNode(int nodeId)
		{
			foreach (StateData sd in states) if (sd.id == nodeId) return sd;
			return null;
		}
		public TransitionData GetTransitionNode(int nodeId)
		{
			foreach (TransitionData td in transitions) if (td.id == nodeId) return td;
			return null;
		}

		public void Load(NonPlayableCharacter t)
		{
			target = t;

			// In case of unfound initialnode
			if (GetStateNode(initialNode) == null) initialNode = states[0].id;

			currentNode = initialNode;
			GetState().OnEnter(target);
		}

		public void Update()
		{
			GetState().Tick(target);
			Follow();
		}

		public void FixedUpdate()
		{
			GetState().FixedTick(target);
		}

		public void Move(int nodeId)
		{
			if (currentNode < 0 || nodeId == currentNode) return;
			GetState().OnExit(target);
			currentNode = nodeId;
			GetState().OnEnter(target);
		}

		public bool IsState(int id)
		{
			foreach(StateData sd in states) if (sd.id == id) return true;
			return false;
		}
		public bool IsTransition(int id)
		{
			foreach (TransitionData td in transitions) if (td.id == id) return true;
			return false;
		}

		public void Follow()
		{
			if (GetStateNode(currentNode).exits.Length == 0) return;
			int exit = GetStateNode(currentNode).exits[0];

			if (IsState(exit))
			{
				Move(exit);
				return;
			}
			else if(IsTransition(exit))
			{
				TransitionData transition = GetTransitionNode(exit);
				
				for(; ;)
				{
					if (transition.disable) return;
					bool check = true;
					foreach(int c in transition.conditions)
					{
						Condition cond = Game.i.library.npcLibrary.GetConditionObject(c);
						if (cond == null)
						{
							check = false;
						}
						else if (!cond.Check(GetState(), target))
						{
							check = false;
						}
					}

					if(check)
					{
						if(IsState(transition.exits[0]))
						{
							Move(transition.exits[0]);
							return;
						}
						else if(IsTransition(transition.exits[0]))
						{
							transition = GetTransitionNode(transition.exits[0]);
						}
					}
					else
					{
						if (IsState(transition.exits[1]))
						{
							Move(transition.exits[1]);
							return;
						}
						else if (IsTransition(transition.exits[1]))
						{
							transition = GetTransitionNode(transition.exits[1]);
						}
					}
				}
			}
		}
	}
}
