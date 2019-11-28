using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Behavior.Editor;

namespace Behavior
{
    [CreateAssetMenu(menuName = "Behavior/AIState")]
    public class AIState : ScriptableObject, INodeable
    {
        public AIAction[] onFixed;
        public AIAction[] onUpdate;
        public AIAction[] onEnter;
        public AIAction[] onExit;

        [HideInInspector] public int id;

        BehaviorGraph graph;

        public void SetGraph(BehaviorGraph graph)
        {
            this.graph = graph;
            id = Mathf.FloorToInt(Random.value * int.MaxValue);
        }

        public void OnEnter()
        {
            ExecuteActions(onEnter);
        }
	
		public void FixedTick()
		{
			ExecuteActions(onFixed);
		}

        public void Tick()
        {
            ExecuteActions(onUpdate);
        }

        public void OnExit()
        {
            ExecuteActions(onExit);
        }
        
        public void ExecuteActions(AIAction[] l)
        {
            foreach(var action in l) {
                if (action == null) continue;
                action.Execute(graph.GetTarget());
            }
        }

        public NonPlayableCharacter GetGraphTarget()
        {
            return graph.GetTarget();
        }
    }
}
