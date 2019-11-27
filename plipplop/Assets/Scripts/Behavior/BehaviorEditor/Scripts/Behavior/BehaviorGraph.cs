using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Behavior.Editor
{
    [CreateAssetMenu(menuName = "Behavior/Graph")]
    public class BehaviorGraph : ScriptableObject
    {
        [SerializeField] public List<Node> nodes = new List<Node>();
        [SerializeField] public int idCount;
        [SerializeField] public Dictionary<AIState, AIStateTransition> transitions = new Dictionary<AIState, AIStateTransition>();
        public AIState initialState;
        List<int> indexToDelete = new List<int>();

        // Runtime
        AIState currentAIState;
        NonPlayableCharacter target;

        #region Checks
        public Node GetNodeWithIndex(int index)
        {
            for (int i = 0; i < nodes.Count; i++) {
                if (nodes[i].id == index)
                    return nodes[i];
            }
            return null;
        }

        public bool AreSomeNodesPendingDeletion()
        {
            return indexToDelete.Count > 0;
        }

        public void DeleteWindowsThatNeedTo()
        {
            for (int i = 0; i < indexToDelete.Count; i++) {
                Node b = GetNodeWithIndex(indexToDelete[i]);
                if (b != null)
                    nodes.Remove(b);
            }

            indexToDelete.Clear();
        }

        public void DeleteNode(int index)
        {
            indexToDelete.AddUnique(index);
        }

        public bool IsAIStateDuplicate(Node b)
        {
            for (int i = 0; i < nodes.Count; i++) {
                if (nodes[i].id == b.id)
                    continue;

                if (b.stateRef.currentAIState != null && nodes[i].stateRef.currentAIState == b.stateRef.currentAIState &&
                    !nodes[i].isDuplicate)
                    return true;
            }
            return false;
        }
        #endregion

        public void Start()
        {
            currentAIState.OnEnter();
        }

        public void Update()
        {
            currentAIState.Tick();
        }

        public void FixedUpdate()
        {
            currentAIState.FixedTick();
            CheckAndFollowTransition();
        }

        public void CheckAndFollowTransition()
        {
            if (!transitions.ContainsKey(currentAIState)) return;

            var transition = transitions[currentAIState];

            // Following the path of conditions until we hit a state, or something empty
            for(; ;) {
                if (transition.disable) return;
                bool check = true;
                foreach (Condition c in transition.conditions) {
                    if (!c.Check(currentAIState)) check = false;
                }
                INodeable nextItem;
                if (check) {
                    nextItem = transition.outputIfTrue;
                }
                else {
                    nextItem = transition.outputIfFalse;
                }
                if (nextItem == null) {
                    Debug.LogError("AIState " + currentAIState.name + " transitions to a NULL state after condition check");
                }
                else {
                    if (nextItem is AIState) {
                        GoToState((AIState)nextItem);
                        return;
                    }
                    else {
                        transition = (AIStateTransition)nextItem;
                    }
                }
            }
        }

        public void GoToState(AIState state)
        {
            currentAIState.OnExit();
            currentAIState = state;
            currentAIState.OnEnter();
        }

        public string GetCurrentAIStateName()
        {
            return currentAIState ? currentAIState.name : "<NULL>";
        }

        public int? GetCurrentAIStateID()
        {
            return currentAIState ? (int?)currentAIState.id : null;
        }

        public void SetTarget(NonPlayableCharacter target)
        {
            this.target = target;
            foreach(var node in nodes) {
                if (node.stateRef.currentAIState != null) {
                    node.stateRef.currentAIState.SetGraph(this);
                }
            }
        }

        public NonPlayableCharacter GetTarget()
        {
            return target;
        }

        public AIStateTransition AddTransition(AIState state)
        {
            if (HasTransition(state)) return GetTransition(state);

            AIStateTransition retVal = new AIStateTransition();
            transitions[state] = retVal;
            retVal.id = idCount;
            idCount++;
            return retVal;
        }

        public bool HasTransition(AIState state)
        {
            return GetTransition(state) != null;
        }

        public AIStateTransition GetTransition(AIState state)
        {
            return transitions.ContainsKey(state) ? transitions[state] : null;
        }

        public void RemoveTransition(AIState state)
        {
            transitions[state] = null;
        }
    }
}
