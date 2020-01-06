using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Behavior.Editor;

namespace Behavior
{
    [CreateAssetMenu(menuName = "Behavior/AIState")]
	public class AIState : ScriptableObject
    {
        public AIAction[] onFixed;
        public AIAction[] onUpdate;
        public AIAction[] onEnter;
        public AIAction[] onExit;

        [HideInInspector] public int id;
		
        public void OnEnter(NonPlayableCharacter target)
        {
            ExecuteActions(onEnter, target);
        }
	
		public void FixedTick(NonPlayableCharacter target)
		{
			ExecuteActions(onFixed, target);
		}

        public void Tick(NonPlayableCharacter target)
        {
			ExecuteActions(onUpdate, target);
        }

        public void OnExit(NonPlayableCharacter target)
        {
            ExecuteActions(onExit, target);
        }
        
        public void ExecuteActions(AIAction[] l, NonPlayableCharacter target)
        {
			foreach (var action in l)
			{
				if (action == null) continue;
                action.Execute(target);
            }
        }
    }
}
