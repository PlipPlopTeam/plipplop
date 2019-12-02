using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Behavior.Editor
{
    [CreateAssetMenu(menuName ="Behavior/Editor/Settings")]
    public class Settings : ScriptableObject
    {
        public BehaviorGraph currentGraph;
        public AIStateDrawNode stateNode;
        public TransitionDrawNode transitionNode;
        public bool MAKE_TRANSITION;
        public bool TRANSITION_TYPE = false;
        public GUISkin skin;
		public GUISkin activeSkin;
        
        public Node AddNodeOnGraph(DrawNode type, float width,float height, string title, Vector3 pos)
        {
            Node baseNode = type is TransitionDrawNode ? (Node)new AIStateTransitionNode() : (Node)new AIStateNode();
            baseNode.drawNode = type;
            baseNode.optimalWidth = width;
            baseNode.optimalHeight = height;
            baseNode.windowTitle = title;
            baseNode.windowRect.x = pos.x;
            baseNode.windowRect.y = pos.y;
            baseNode.RefreshRectSize();

            if (baseNode is AIStateNode) {
                currentGraph.stateNodes.Add((AIStateNode)baseNode);
            }
            else if (baseNode is AIStateTransitionNode) {
                currentGraph.transitionNodes.Add((AIStateTransitionNode)baseNode);
            }
            baseNode.id = currentGraph.idCount;
            currentGraph.idCount++;

            return baseNode;
        }
    }
}
