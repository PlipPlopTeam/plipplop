using System.Collections.Generic;
using UnityEngine;

namespace Behavior.Editor
{
    [System.Serializable]
    public abstract class Node
    {
        [System.Serializable]
        public class Reroute
        {
            public readonly int exitIndex;
            public readonly int beaconIndex;
            public Vector2 position;

            public Reroute(int exitIndex, int beaconIndex)
            {
                this.exitIndex = exitIndex;
                this.beaconIndex = beaconIndex;
            }
        }

        public int id;
        public Rect windowRect;
        public string windowTitle;
        public float optimalWidth = 100f;
        public float optimalHeight = 100f;
        public string comment;
        public bool isAssigned;
		public bool showDescription;
		public bool isOnCurrent;
        public BehaviorGraph graph;
        public List<Reroute> reroutes = new List<Reroute>();

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

		public bool IsStartNode()
        {
            return id == BehaviorGraph.startNodeId;
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

        public Reroute AddReroute(int exitIndex)
        {
            int beaconIndex = -1;
            foreach (var reroute in GetReroutes(exitIndex)) {
                if (reroute.beaconIndex > beaconIndex) {
                    beaconIndex = reroute.beaconIndex;
                }
            }
            beaconIndex++;

            reroutes.Add(
                new Reroute(
                    exitIndex: exitIndex,
                    beaconIndex: beaconIndex
                )
            );
            return reroutes[reroutes.Count - 1];
        }

        public void DeleteReroute(int exitIndex, int beaconIndex)
        {
            reroutes.RemoveAll(o=> exitIndex == o.exitIndex && beaconIndex == o.beaconIndex);
        }

        public List<Reroute> GetReroutes(int exitIndex)
        {
            var list = reroutes.FindAll(o => o.exitIndex == exitIndex);
            list.Sort((a, b) => -a.beaconIndex.CompareTo(b.beaconIndex));

            return list;
		}

#if UNITY_EDITOR
		public DrawNode drawNode;
		public void DrawWindow()
		{
			if (drawNode != null)
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
#endif
	}
}