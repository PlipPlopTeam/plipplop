﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Behavior.Editor
{
    public class TransitionDrawNode : DrawNode
    {
        public void Init(AIStateDrawNode enterAIState, AIStateTransitionNode transition)
        {
            //      this.enterAIState = enterAIState;
        }

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
                b.isAssigned = false;
            }
            else
            {
                b.optimalHeight = 40 + transitionNode.conditions.Count * 22;
                b.isAssigned = true;

                for (int i = 0; i < b.exitNodes.Count; i++) {
                    if (!b.exitNodes[i].HasValue) continue;

                    Node targetNode = BehaviorEditor.settings.currentGraph.GetNodeWithIndex(b.exitNodes[i]);
                    if (targetNode != null) {
                        transitionNode.SetExitNode(i, targetNode.id);
                    }
                    else {
                        transitionNode.RemoveExitNode(i);
                    }
                }
			}
            
        }

    }
}