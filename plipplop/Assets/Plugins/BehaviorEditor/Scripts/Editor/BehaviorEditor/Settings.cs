using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PP.Behavior
{
    [CreateAssetMenu(menuName ="Behavior/Editor/Settings")]
    public class Settings : ScriptableObject
    {
        public BehaviorGraph currentGraph;
        public StateNode stateNode;
		public PortalNode portalNode;
        public TransitionNode transitionNode;
        public bool MAKE_TRANSITION;
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
            baseNode.stateRef = new StateNodeReferences();
            baseNode.id = currentGraph.idCount;
            currentGraph.idCount++;
            return baseNode;
        }
    }
}
