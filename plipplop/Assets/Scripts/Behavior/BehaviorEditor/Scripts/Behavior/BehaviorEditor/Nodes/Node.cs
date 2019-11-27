using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Behavior.Editor
{
    [System.Serializable]
    public class Node
    {
        public int id;
        public DrawNode drawNode;
        public Rect windowRect;
        public string windowTitle;
        public int enterNode;
        public int[] exitNodes;
        public bool isDuplicate;
        public string comment;
        public bool isAssigned;
		public bool showDescription;
		public bool isOnCurrent;

        public bool collapse;
		public bool showActions = true;
		public bool showEnterExit = false;
        [HideInInspector]
        public bool previousCollapse;

        [SerializeField]
        public AIStateNodeReferences stateRef;
        [SerializeField]
        public TransitionNodeReferences transRef;

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

    }

    [System.Serializable]
    public class AIStateNodeReferences
    { 
    //    [HideInInspector]
        public AIState currentAIState;
        [HideInInspector]
        public AIState previousAIState;
		public SerializedObject serializedAIState;
	    public ReorderableList onFixedList;
		public ReorderableList onUpdateList;
		public ReorderableList onEnterList;
		public ReorderableList onExitList;
	}

	[System.Serializable]
    public class TransitionNodeReferences
    {
        [HideInInspector]
        public Condition previousCondition;
        public int transitionId;
    }
}
