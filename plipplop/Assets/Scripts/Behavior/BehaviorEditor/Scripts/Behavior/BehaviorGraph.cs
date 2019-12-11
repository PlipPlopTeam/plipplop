using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using UnityEngine;


namespace Behavior.Editor
{
    [CreateAssetMenu(menuName = "Behavior/Graph")]
    public class BehaviorGraph : ScriptableObject
    {
		public const int startNodeId = -1;
		// This part will be saved
		[SerializeField] public List<AIStateNode> stateNodes = new List<AIStateNode>();
        [SerializeField] public List<AIStateTransitionNode> transitionNodes = new List<AIStateTransitionNode>();
        [SerializeField] public int idCount;
        [SerializeField] public int initialStateID;
        [SerializeField] public Vector2 editorScrollPosition;
        // These are helpers
        [HideInInspector] public List<AIStateTransitionNode> transitions { get { return nodes.Where(o => { return (o is AIStateTransitionNode); }).Select(o => { return (AIStateTransitionNode)o; }).ToList(); } }
        [HideInInspector] public ReadOnlyCollection<Node> nodes { get{ return stateNodes.Select(o => { return (Node)o; }).Concat(transitionNodes.Select(o => { return (Node)o; })).ToList().AsReadOnly(); } }
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
                Debug.Log("Deleting node " + i + " out of " + indexToDelete.Count + " (id: "+ indexToDelete[i]+ "<>" + (b != null ? b.id.ToString()+"_"+b.windowTitle : "??") + ")");   
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
            Debug.Log("Requested to delete node with index #" + index);
            indexToDelete.AddUnique(index);
        }

        public bool IsAIStateDuplicate(AIStateNode b)
        {
            foreach(var node in nodes.Where(o=>o is AIStateNode)) {

                var aiNode = (AIStateNode)node;
                if (aiNode.id == b.id) continue;

                if (b.state != null && aiNode.state == b.state)
                    return true;
            }

            return false;
        }

        public AIState GetCurrentAIState()
        {
            return currentStateNode != null ? currentStateNode.state : null;
        }

        public void Start()
        {
            currentStateNode = (AIStateNode) GetNodeWithIndex(startNodeId);
			currentStateNode.state = GetInitialAIState();
			GetCurrentAIState().OnEnter(target);
		}

		public void Update()
        {
            GetCurrentAIState().Tick(target);
		}

		public void FixedUpdate()
        {
			GetCurrentAIState().FixedTick(target);
            CheckAndFollowTransition();
        }

        public void CheckAndFollowTransition()
        {
			// IF THE CURRENT HAS AN EXIT
			if (currentStateNode.exitNodes.Count == 0) return;
			var node = GetNodeWithIndex(currentStateNode.exitNodes[0]);
		
			// If the exit node is another state
			if(node is AIStateNode)
			{
				GoToNode((AIStateNode)node);
				return;
			}

			// If the exit node is a transition
			if (!(node is AIStateTransitionNode)) return;
			var transition = (AIStateTransitionNode)node;

			// Following the path of conditions until we hit a state, or somESubject empty
			for (; ;)
			{
                if (transition.disable) return;

                bool check = true;
                foreach (Condition c in transition.conditions)
				{
                    if (c == null) continue; // No condition set
                    if (!c.Check(GetCurrentAIState(), target)) check = false;
                }

                Node nextItem;
                if (check)
				{
                    nextItem = GetNodeWithIndex(transition.exitNodes[0]);
                }
                else
				{
                    nextItem = GetNodeWithIndex(transition.exitNodes[1]);
				}
                if (nextItem == null)
				{
					throw new System.Exception("Node " + transition.id + " transitions to a NULL state after condition check");
                }
                else
				{
                    if (nextItem is AIStateNode)
					{
                        GoToNode((AIStateNode)nextItem);
                        return;
                    }
                    else
					{
                        transition = (AIStateTransitionNode)nextItem;
                    }
                }
            }
        }

        public void GoToNode(AIStateNode node)
        {
			if (currentStateNode == node) return;
			GetCurrentAIState().OnExit(target);
            currentStateNode = node;
            GetCurrentAIState().OnEnter(target);
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
        }

        public NonPlayableCharacter GetTarget()
        {
            return target;
        }

        public AIStateTransitionNode AddTransition(AIStateNode node)
        {
            if (HasTransition(node)) return GetTransition(node);

            AIStateTransitionNode retVal = new AIStateTransitionNode();
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


            node.SetExitNode(index, retVal.id);

            return retVal;
        }

        public bool HasTransition(AIStateNode node)
        {
            return GetTransition(node) != null;
        }

        public AIStateTransitionNode GetTransition(AIStateNode node)
        {
            return transitions.Find(o => o.id == node.exitNodes[0]);
        }

        public AIStateTransitionNode GetTransition(int i)
        {
            return transitions.Find(o => o.id == i);
        }

        public AIState GetInitialAIState()
        {
            try {
                return Game.i.library.npcLibrary.aiStates.Find(o => o.id == initialStateID).resource;
            }
            catch (System.NullReferenceException) {
                Debug.LogError("!! COULD NOT GET the initial state of graph " + name + " (state id: " + initialStateID + ").\nCHECK THE LIBRARY!");
                return null;
            }
        }
    }
}
