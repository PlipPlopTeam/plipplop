using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Behavior.Editor
{
    [CreateAssetMenu(menuName ="Behavior/Editor/Settings")]
    public class Settings : ScriptableObject
    {
        public BehaviorGraph currentGraph;
        public AIStateNode stateNode;
		public PortalNode portalNode;
        public TransitionNode transitionNode;
        public bool MAKE_TRANSITION;
        public bool TRANSITION_TYPE = false;
        public GUISkin skin;
		public GUISkin activeSkin;
        
        public Node AddNodeOnGraph(DrawNode type, float width,float height, string title, Vector3 pos)
        {
            Node baseNode = new Node();
            baseNode.drawNode = type;
            baseNode.windowRect.width = width;
            baseNode.windowRect.height = height;
            baseNode.windowTitle = title;
            baseNode.windowRect.x = pos.x;
            baseNode.windowRect.y = pos.y;
            currentGraph.nodes.Add(baseNode);
            baseNode.transRef = new TransitionNodeReferences();
            baseNode.stateRef = new AIStateNodeReferences();
            baseNode.id = currentGraph.idCount;
            currentGraph.idCount++;

            if (type is TransitionNode) {
                baseNode.exitNodes = new int[2];
            }
            else {
                baseNode.exitNodes = new int[1];
            }
            return baseNode;
        }
    }
}
