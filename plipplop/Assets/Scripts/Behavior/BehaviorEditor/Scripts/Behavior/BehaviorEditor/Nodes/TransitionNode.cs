using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Behavior.Editor
{
    public class TransitionNode : DrawNode
    {

        readonly int heightBetweenOutputs = 20;

        public void Init(AIStateNode enterAIState, AIStateTransition transition)
        {
            //      this.enterAIState = enterAIState;
        }

        public override void DrawWindow(Node b)
        {
            Node enterNode = BehaviorEditor.settings.currentGraph.GetNodeWithIndex(b.enterNode);
			if (enterNode == null) {
                EditorGUILayout.LabelField(@"/!\ This node has no entry node");
                return;
			}
			
			if (enterNode.stateRef.currentAIState == null)
			{
				BehaviorEditor.settings.currentGraph.DeleteNode(b.id);
				return;
			}

            AIStateTransition transition = enterNode.stateRef.currentAIState.GetTransition();

			if (transition == null)
				return;

            for(int i = 0; i < transition.conditions.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                transition.conditions[i] = (Condition)EditorGUILayout.ObjectField(transition.conditions[i] , typeof(Condition), false);
                if(GUILayout.Button("-")) transition.conditions.RemoveAt(i);
                EditorGUILayout.EndHorizontal();
            }

            if(GUILayout.Button("Add")) transition.conditions.Add(null);

            if(transition.conditions.Count == 0)
            {     
                b.windowRect.height = 60;       
                EditorGUILayout.LabelField("No Condition!");
                b.isAssigned = false;
            }
            else
            {
                b.windowRect.height = 40 + transition.conditions.Count * 22;
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
                                if (i == 0) transition.outputIfTrue = null;
                                else transition.outputIfFalse = null;
                            else {
                                if (i == 0) transition.outputIfTrue = targetNode.stateRef.currentAIState;
                                else transition.outputIfFalse = targetNode.stateRef.currentAIState;
                            }
                        }
                        else {
                            if (i == 0) transition.outputIfTrue = null;
                            else transition.outputIfFalse = null;
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


            for (int i = 0; i < 2; i++) {
                Node targetNode = BehaviorEditor.settings.currentGraph.GetNodeWithIndex(b.exitNodes[i]);
                if (targetNode != null) {
                    rect = b.windowRect;
                    //rect.x += rect.width;
                    Rect endRect = targetNode.windowRect;
                    //endRect.x -= endRect.width * .5f;

                    Color targetColor = Color.white;

                    if (targetNode.drawNode is AIStateNode) {
                        if (!targetNode.isAssigned || targetNode.isDuplicate) targetColor = Color.red;
                    }
                    else {
                        if (!targetNode.isAssigned) targetColor = Color.grey;
                        else targetColor = Color.white;
                    }

                    //rect.position = new Vector2(rect.position.x - rect.size.x, rect.position.y);
                    BehaviorEditor.DrawNodeCurve(rect, endRect, false, Color.white);
                }
            }
        }
    }
}
