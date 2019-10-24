using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using PP;

namespace PP.Behavior
{
    public class TransitionNode : DrawNode
    {
      
        public void Init(StateNode enterState, Transition transition)
        {
      //      this.enterState = enterState;
        }

        public override void DrawWindow(Node b)
        {
            Node enterNode = BehaviorEditor.settings.currentGraph.GetNodeWithIndex(b.enterNode);
			if (enterNode == null)
			{
				return;
			}
			
			if (enterNode.stateRef.currentState == null)
			{
				BehaviorEditor.settings.currentGraph.DeleteNode(b.id);
				return;
			}

            Transition transition = enterNode.stateRef.currentState.GetTransition(b.transRef.transitionId);

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

					Node targetNode = BehaviorEditor.settings.currentGraph.GetNodeWithIndex(b.targetNode);
					if (targetNode != null)
					{
						if (!targetNode.isDuplicate)
							transition.targetState = targetNode.stateRef.currentState;
						else
							transition.targetState = null;
					}
					else
					{
						transition.targetState = null;
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
            rect.y += b.windowRect.height * .5f;
            rect.width = 1;
            rect.height = 1;

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
                BehaviorEditor.DrawNodeCurve(r, rect, true, targetColor);
            }

            if (b.isDuplicate)
                return;

            if(b.targetNode > 0)
            {
                Node t = BehaviorEditor.settings.currentGraph.GetNodeWithIndex(b.targetNode);
                if (t == null)
                {
                    b.targetNode = -1;
                }
                else
                {
                    rect = b.windowRect;
                    rect.x += rect.width;
                    Rect endRect = t.windowRect;
                    endRect.x -= endRect.width * .5f;

                    Color targetColor = Color.white;

					if (t.drawNode is StateNode)
					{
						if (!t.isAssigned || t.isDuplicate) targetColor = Color.red;
					}
					else
					{
						if (!t.isAssigned) targetColor = Color.grey;
						else targetColor = Color.white;
					}
                    
                    BehaviorEditor.DrawNodeCurve(rect, endRect, false, targetColor);
                }

            }
        }
    }
}
