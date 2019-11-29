using System.Collections;
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

            Node enterNode = BehaviorEditor.settings.currentGraph.GetNodeWithIndex(b.enterNode);
			if (enterNode == null) {
                EditorGUILayout.LabelField(@"/!\ This node has no entry node");
			}


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
                b.windowRect.height = 40 + transitionNode.conditions.Count * 22;
                b.isAssigned = true;
				if (b.isDuplicate)
				{
					EditorGUILayout.LabelField("Duplicate Condition!");
				}
				else
				{
                    //GUILayout.Label(transition.condition.description);

                    for (int i = 0; i < 2; i++) {
                        Node targetNode = BehaviorEditor.settings.currentGraph.GetNodeWithIndex(b.exitNodes[i]);
                        if (targetNode != null) {
                            if (targetNode.isDuplicate)
                                transitionNode.RemoveExitNode(i);
                            else {
                                transitionNode.SetExitNode(i, targetNode.id);
                            }
                        }
                        else {
                            transitionNode.RemoveExitNode(i);
                        }
                    }
				}
			}
            
            /*
            if (b.transRef.previousCondition != transition.condition)
            {
                b.transRef.previousCondition = transition.condition;
                b.isDuplicate = BehaviorEditor.settings.currentGraph.IsTransitionDuplicate(b);
				
				if (!b.isDuplicate)
                {
					BehaviorEditor.forceSetDirty = true;
					// BehaviorEditor.settings.currentGraph.SetNode(this);   
				}
            }
            */
        }

        public override void DrawCurve(Node b)
        {
            Rect rect = b.windowRect.Shift(-BehaviorEditor.scrollPos);
            //rect.y += b.windowRect.height * .5f;
            //rect.position = new Vector2(rect.position.x - rect.size.x, rect.position.y);
            //rect.width = 1;
            //rect.height = 1;

            Node e = BehaviorEditor.settings.currentGraph.GetNodeWithIndex(b.enterNode);
            if (e == null)
            {
                // Do nothing
            }
            else
            {
                Color targetColor = Color.white;
                if (!b.isAssigned || b.isDuplicate)
                    targetColor = Color.grey;

                Rect r = e.windowRect.Shift(-BehaviorEditor.scrollPos);
                BehaviorEditor.DrawNodeCurve(r, rect, Color.white);
            }

            if (b.isDuplicate)
                return;


            for (int i = 0; i < b.exitNodes.Count; i++) {
                if (b.exitNodes[i] == null) continue;

                Node targetNode = BehaviorEditor.settings.currentGraph.GetNodeWithIndex(b.exitNodes[i]);
                if (targetNode != null) {
                    rect = b.windowRect.Shift(-BehaviorEditor.scrollPos);
                    //rect.x += rect.width;
                    Rect endRect = targetNode.windowRect.Shift(-BehaviorEditor.scrollPos);
                    //endRect.x -= endRect.width * .5f;

                    Color targetColor = Color.white;

                    if (i == 0) targetColor = Color.green;
                    if (i == 1) targetColor = Color.red;

                    //rect.position = new Vector2(rect.position.x - rect.size.x, rect.position.y);
                    BehaviorEditor.DrawNodeCurve(rect, endRect, targetColor, i+1);
                }
            }
        }
    }
}
