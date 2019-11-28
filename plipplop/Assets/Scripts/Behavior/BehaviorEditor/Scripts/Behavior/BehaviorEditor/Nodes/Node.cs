using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Behavior.Editor
{
    [System.Serializable]
    public abstract class Node
    {
        public int id;
        public DrawNode drawNode;
        public Rect windowRect;
        public string windowTitle;
        public int? enterNode;
        public int?[] exitNodes;
        public bool isDuplicate;
        public string comment;
        public bool isAssigned;
		public bool showDescription;
		public bool isOnCurrent;
        public BehaviorGraph graph;

        public bool collapse;
		public bool showActions = true;
		public bool showEnterExit = false;
        [HideInInspector]
        public bool previousCollapse;

        public void DrawWindow()
        {
            if(drawNode != null)
            {
                drawNode.DrawWindow(this);
            }
        }

        public void DrawCurve()
        {
            if (drawNode != null)
            {
                drawNode.DrawCurve(this);
            }
        }

        public bool IsStartNode()
        {
            return id == BehaviorEditor.startNodeId;
        }

        public virtual void SetGraph(BehaviorGraph graph)
        {
            this.graph = graph;
        }
    }
}
