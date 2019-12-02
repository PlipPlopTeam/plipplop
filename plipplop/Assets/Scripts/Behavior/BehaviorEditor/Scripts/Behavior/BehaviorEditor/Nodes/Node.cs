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
        public float optimalWidth = 100f;
        public float optimalHeight = 100f;
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

        // External
        public System.Collections.ObjectModel.ReadOnlyCollection<int?> exitNodes {
            get {
                List<int?> nodes = new List<int?>();
                for (int i = 0; i < s_hasExitNodes.Length; i++) {
                    var hasNode = s_hasExitNodes[i];
                    if (hasNode) {
                        nodes.Add(s_exitNodes[i]);
                    }
                    else {
                        nodes.Add(null);
                    }
                }
                return nodes.AsReadOnly();
            }
            set {
                for (int i = 0; i < value.Count; i++) {
                    if (value[i] == null) {
                        s_hasExitNodes[i] = false;
                    }
                    else {
                        s_hasExitNodes[i] = true;
                        s_exitNodes[i] = value[i].Value;
                    }
                }
            }
        }

        // Internal - used for serialization
        [SerializeField] int[] s_exitNodes;
        [SerializeField] bool[] s_hasExitNodes;

        public void SetExitNode(int i, int value)
        {
            List<int?> nodes = new List<int?>(exitNodes);
            nodes[i] = value;
            SetExitNodes(nodes);
        }

        public void RemoveExitNode(int i)
        {
            List<int?> nodes = new List<int?>(exitNodes);
            nodes[i] = null;
            SetExitNodes(nodes);
        }

        public void ClearExitNodes()
        {
            for(var i = 0; i < exitNodes.Count; i++) {
                RemoveExitNode(i);
            }
        }

        public void SetExitNodes(IEnumerable<int?> exitNodes)
        {
            this.exitNodes = new List<int?>(exitNodes).AsReadOnly();
        }

        internal void SetExitNodesNumber(int amount)
        {
            s_exitNodes = new int[amount];
            s_hasExitNodes = new bool[amount];
        }

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

        public void RefreshRectSize(float zoom=1f)
        {
            windowRect.width = optimalWidth * zoom;
            windowRect.height = optimalHeight * zoom;
        }
    }
}
