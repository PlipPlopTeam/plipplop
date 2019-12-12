using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Behavior.Editor
{
    public class TransitionDrawNode : DrawNode
    {
		#if UNITY_EDITOR
		public override void DrawWindow(Node b)
        {
            if (!(b is AIStateTransitionNode)) {
                Debug.LogError("Attempted to draw a non-transition node with a TransitionDrawNode");
                return;
            }

            // Window
            AIStateTransitionNode transitionNode = (AIStateTransitionNode)b;

            for(int i = 0; i < transitionNode.conditions.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                transitionNode.conditions[i] = (Condition)EditorGUILayout.ObjectField(transitionNode.conditions[i] , typeof(Condition), false);
                if(GUILayout.Button("-")) transitionNode.conditions.RemoveAt(i);
                EditorGUILayout.EndHorizontal();
            }

            if(GUILayout.Button("Add")) transitionNode.conditions.Add(null);

            if(transitionNode.conditions.Count == 0)
            {     
                b.windowRect.height = 60;       
                EditorGUILayout.LabelField("No Condition!");
            }
            else
            {
                b.optimalHeight = 40 + transitionNode.conditions.Count * 22;

                for (int i = 0; i < b.exitNodes.Count; i++) {
                    if (!b.exitNodes[i].HasValue) continue;

                    Node targetNode = BehaviorEditor.currentGraph.GetNodeWithIndex(b.exitNodes[i]);
                    if (targetNode != null) {
                        transitionNode.SetExitNode(i, targetNode.id);
                    }
                    else {
                        transitionNode.RemoveExitNode(i);
                    }
                }
			}
		}
		#endif
	}
}
