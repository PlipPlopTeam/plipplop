using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Behavior
{
    [CreateAssetMenu(menuName = "Behavior/State")]
    public class State : ScriptableObject
    {
    	public AIAction[] onFixed;
        public AIAction[] onUpdate;
        public AIAction[] onEnter;
        public AIAction[] onExit;

        public int idCount;
		[SerializeField]
        public List<Transition> transitions = new List<Transition>();

        public void OnEnter(StateManager states)
        {
            ExecuteActions(states, onEnter);
        }
	
		public void FixedTick(StateManager states)
		{
			ExecuteActions(states,onFixed);
		}

        public void Tick(StateManager states)
        {
            ExecuteActions(states, onUpdate);
            CheckTransitions(states);
        }

        public void OnExit(StateManager states)
        {
            ExecuteActions(states, onExit);
        }

        public void CheckTransitions(StateManager states)
        {
            for (int i = 0; i < transitions.Count; i++)
            {
                if (transitions[i].disable)
                    continue;

                bool check = true;
                foreach(Condition c in transitions[i].conditions)
                {
                    if(!c.Check(states)) check = false;
                }

                if(check)
                {
                    if (transitions[i].targetState != null)
                    {
                        states.currentState = transitions[i].targetState;
                        OnExit(states);
                        states.currentState.OnEnter(states);
                    }
                    return;
                }
            }
        }
        
        public void ExecuteActions(StateManager states, AIAction[] l)
        {
            for (int i = 0; i < l.Length; i++)
            {
                if (l[i] != null)
                    l[i].Execute(states);
            }
        }

        public Transition AddTransition()
        {
            Transition retVal = new Transition();
            transitions.Add(retVal);
            retVal.id = idCount;
            idCount++;
            return retVal;
        }

        public Transition GetTransition(int id)
        {
            for (int i = 0; i < transitions.Count; i++)
            {
                if (transitions[i].id == id)
                    return transitions[i];
            }
            return null;
        }

		public void RemoveTransition(int id)
		{
			Transition t = GetTransition(id);
			if (t != null)
				transitions.Remove(t);
		}
    }
}
