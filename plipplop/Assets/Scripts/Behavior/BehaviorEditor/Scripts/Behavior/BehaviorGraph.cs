using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;


namespace Behavior.Editor
{
    [CreateAssetMenu(menuName = "Behavior/Graph")]
    public class BehaviorGraph : ScriptableObject
    {
        // This part will be saved
        [SerializeField] public List<AIStateNode> stateNodes = new List<AIStateNode>();
        [SerializeField] public List<AIStateTransitionNode> transitionNodes = new List<AIStateTransitionNode>();
        [SerializeField] public int idCount;
        [SerializeField] public Dictionary<AIStateNode, AIStateTransitionNode> stateTransitions = new Dictionary<AIStateNode, AIStateTransitionNode>();
        [SerializeField] public AIState initialState;

        // These are helpers
        public List<AIStateTransitionNode> transitions { get { return nodes.Where(o => { return (o is AIStateTransitionNode); }).Select(o => { return (AIStateTransitionNode)o; }).ToList(); } }
        public ReadOnlyCollection<Node> nodes { get{ return stateNodes.Select(o => { return (Node)o; }).Concat(transitionNodes.Select(o => { return (Node)o; })).ToList().AsReadOnly(); } }
        List<int> indexToDelete = new List<int>();

        // Runtime
        AIStateNode currentStateNode;
        NonPlayableCharacter target;

        public Node GetNodeWithIndex(int? index)
        {
            if (index == null) return null;

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
                if (b != null) {
                    if (b is AIStateNode) {
                        stateNodes.RemoveAll(o => o.id == b.id);
                    }
                    else {
                        transitionNodes.RemoveAll(o => o.id == b.id);
                    }
                }
            }

            indexToDelete.Clear();
        }

        public void DeleteNode(int index)
        {
            indexToDelete.AddUnique(index);
        }

        public bool IsAIStateDuplicate(AIStateNode b)
        {
            foreach(var node in nodes.Where(o=>o is AIStateNode)) {

                var aiNode = (AIStateNode)node;
                if (aiNode.id == b.id) continue;

                if (b.currentAIState != null && aiNode.currentAIState == b.currentAIState &&
                    !aiNode.isDuplicate)
                    return true;
            }

            return false;
        }

        public AIState GetCurrentAIState()
        { 
            return currentStateNode.currentAIState;
        }

        public void Start()
        {
            GetCurrentAIState().OnEnter();
        }

        public void Update()
        {
            GetCurrentAIState().Tick();
        }

        public void FixedUpdate()
        {
            GetCurrentAIState().FixedTick();
            CheckAndFollowTransition();
        }

        public void CheckAndFollowTransition()
        {
            if (!stateTransitions.ContainsKey(currentStateNode)) return;

            var transition = stateTransitions[currentStateNode];

            // Following the path of conditions until we hit a state, or something empty
            for(; ;) {
                if (transition.disable) return;
                bool check = true;
                foreach (Condition c in transition.conditions) {
                    if (!c.Check(GetCurrentAIState())) check = false;
                }
                Node nextItem;
                if (check) {
                    nextItem = transition.outputIfTrue;
                }
                else {
                    nextItem = transition.outputIfFalse;
                }
                if (nextItem == null) {
                    Debug.LogError("Node " + nextItem + " transitions to a NULL state after condition check");
                }
                else {
                    if (nextItem is AIStateNode) {
                        GoToNode((AIStateNode)nextItem);
                        return;
                    }
                    else {
                        var nextTransition = (AIStateTransitionNode)nextItem;
                       // transition = nextTransition.transRef.
                    }
                }
            }
        }

        public void GoToNode(AIStateNode node)
        {
            GetCurrentAIState().OnExit();
            currentStateNode = node;
            GetCurrentAIState().OnEnter();
        }

        public string GetCurrentAIStateName()
        {
            return GetCurrentAIState() ? GetCurrentAIState().name : "<NULL>";
        }

        public int? GetCurrentAIStateID()
        {
            return GetCurrentAIState() ? (int?)GetCurrentAIState().id : null;
        }

        public void SetTarget(NonPlayableCharacter target)
        {
            this.target = target;
            foreach(var node in nodes) {
                node.SetGraph(this);
            }
        }

        public NonPlayableCharacter GetTarget()
        {
            return target;
        }

        public AIStateTransitionNode AddTransition(AIStateNode node)
        {
            if (HasTransition(node)) return GetTransition(node);

            AIStateTransitionNode retVal = new AIStateTransitionNode();
            stateTransitions[node] = retVal;
            retVal.id = idCount;
            idCount++;
            return retVal;
        }

        public AIStateTransitionNode AddTransition(AIStateTransitionNode node, int index)
        {
            if (index == 0 && node.outputIfTrue is AIStateTransitionNode) return (AIStateTransitionNode)node.outputIfTrue;
            if (index == 1 && node.outputIfFalse is AIStateTransitionNode) return (AIStateTransitionNode)node.outputIfFalse;

            AIStateTransitionNode retVal = new AIStateTransitionNode();
            retVal.id = idCount;
            idCount++;

            node.exitNodes[index] = retVal.id;

            return retVal;
        }

        public bool HasTransition(AIStateNode node)
        {
            return GetTransition(node) != null;
        }

        public AIStateTransitionNode GetTransition(AIStateNode node)
        {
            return stateTransitions.ContainsKey(node) ? stateTransitions[node] : null;
        }

        public AIStateTransitionNode GetTransition(int i)
        {
            return transitions.Find(o => o.id == i);
        }

        public void RemoveTransition(AIStateNode node)
        {
            stateTransitions[node] = null;
        }
    }
}
