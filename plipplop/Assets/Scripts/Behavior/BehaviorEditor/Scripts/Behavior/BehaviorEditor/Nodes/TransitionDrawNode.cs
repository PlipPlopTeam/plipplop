using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Behavior.Editor
{
    public class TransitionDrawNode : DrawNode
    {

        readonly int heightBetweenOutputs = 20;

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
                return;
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
                                transitionNode.exitNodes[i] = null;
                            else {
                                transitionNode.exitNodes[i] = targetNode.id;
                            }
                        }
                        else {
                            transitionNode.exitNodes[i] = null;
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
            Rect rect = b.windowRect;
            //rect.y += b.windowRect.height * .5f;
            //rect.position = new Vector2(rect.position.x - rect.size.x, rect.position.y);
            //rect.width = 1;
            //rect.height = 1;

            Node e = BehaviorEditor.settings.currentGraph.GetNodeWithIndex(b.enterNode);
            if (e == null)
            {
                BehaviorEditor.settings.currentGraph.DeleteNode(b.id);
            }
            else
            {
                Color targetColor = Color.white;
                if (!b.isAssigned || b.isDuplicate)
                    targetColor = Color.grey;

                Rect r = e.windowRect;
                BehaviorEditor.DrawNodeCurve(r, rect, true, Color.white);
            }

            if (b.isDuplicate)
                return;


            for (int i = 0; i < b.exitNodes.Length; i++) {
                Node targetNode = BehaviorEditor.settings.currentGraph.GetNodeWithIndex(b.exitNodes[i]);
                if (targetNode != null) {
                    rect = b.windowRect;
                    rect.y += i*heightBetweenOutputs;
                    //rect.x += rect.width;
                    Rect endRect = targetNode.windowRect;
                    //endRect.x -= endRect.width * .5f;

                    Color targetColor = Color.white;

                    if (i == 0) targetColor = Color.red;
                    if (i == 1) targetColor = Color.green;

                    //rect.position = new Vector2(rect.position.x - rect.size.x, rect.position.y);
                    BehaviorEditor.DrawNodeCurve(rect, endRect, false, targetColor);
                }
            }
        }
    }
}
