using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

using System;
using System.IO;

namespace Behavior.Editor
{
    public class AIStateNode : DrawNode
    {
        public override void DrawWindow(Node b)
        {
            if (b.IsStartNode()) {
                EditorGUILayout.LabelField("Start node. The behavior will begin from here.");
                try {
                    EditorGUILayout.LabelField("Initial state: " + b.stateRef.currentAIState);
                }
                catch { };
                return;
            }
            if(b.stateRef.currentAIState == null)
            {
                EditorGUILayout.LabelField("Add state to modify:");
            }
            else
            {
                string collapseText = "";
                if(!b.collapse)
                {
                    collapseText = "Collapse";
                }
                else
                {
                    collapseText = "Expand";
                    b.windowRect.height = 65;
                }

                b.collapse = EditorGUILayout.Toggle(collapseText, b.collapse);
            }

            b.stateRef.currentAIState = (AIState)EditorGUILayout.ObjectField(b.stateRef.currentAIState, typeof(AIState), false);

            if(b.previousCollapse != b.collapse)
            {
                b.previousCollapse = b.collapse;
            }

            if(b.stateRef.previousAIState != b.stateRef.currentAIState)
            {
                //b.serializedAIState = null;
                b.isDuplicate = BehaviorEditor.settings.currentGraph.IsAIStateDuplicate(b);
				b.stateRef.previousAIState = b.stateRef.currentAIState;

				if (!b.isDuplicate)
				{
					Vector3 pos = new Vector3(b.windowRect.x,b.windowRect.y,0);
					pos.x += b.windowRect.width * 2;

					SetupReordableLists(b);

                    //Load transtions
                    BehaviorEditor.AddTransitionNodeFromTransition(b.stateRef.currentAIState.GetTransition(), b, pos);

                    BehaviorEditor.forceSetDirty = true;
				}
				
			}

			if (b.isDuplicate)
            {
                EditorGUILayout.LabelField("AIState is a duplicate!");
                b.windowRect.height = 100;
				return;
            }

            if (b.stateRef.currentAIState != null)
            {
                b.isAssigned = true;
                
                if (!b.collapse)
                {
					if (b.stateRef.serializedAIState == null)
					{
						SetupReordableLists(b);

					//	SerializedObject serializedAIState = new SerializedObject(b.stateRef.currentAIState);
					}

					float standard = 100;
					b.stateRef.serializedAIState.Update();
					b.showActions = EditorGUILayout.Toggle("Show Actions ", b.showActions);
					if (b.showActions)
					{
						b.stateRef.onFixedList.DoLayoutList();
						b.stateRef.onUpdateList.DoLayoutList();
						standard += 125 + (b.stateRef.onUpdateList.count + b.stateRef.onFixedList.count) * 18;
					}
					b.showEnterExit = EditorGUILayout.Toggle("Show Enter/Exit ", b.showEnterExit);
					if (b.showEnterExit)
					{
						b.stateRef.onEnterList.DoLayoutList();
						b.stateRef.onExitList.DoLayoutList();
						standard += 125 + (b.stateRef.onEnterList.count + b.stateRef.onExitList.count) * 18;
					}
					b.stateRef.serializedAIState.ApplyModifiedProperties();
                    b.windowRect.height = standard;
                }   
            }
            else
            {
                b.isAssigned = false;
            }
		}

		void SetupReordableLists(Node b)
		{

			b.stateRef.serializedAIState = new SerializedObject(b.stateRef.currentAIState);
			b.stateRef.onFixedList = new ReorderableList(b.stateRef.serializedAIState, b.stateRef.serializedAIState.FindProperty("onFixed"), true, true, true, true);
			b.stateRef.onUpdateList = new ReorderableList(b.stateRef.serializedAIState, b.stateRef.serializedAIState.FindProperty("onUpdate"), true, true, true, true);
			b.stateRef.onEnterList = new ReorderableList(b.stateRef.serializedAIState, b.stateRef.serializedAIState.FindProperty("onEnter"), true, true, true, true);
			b.stateRef.onExitList = new ReorderableList(b.stateRef.serializedAIState, b.stateRef.serializedAIState.FindProperty("onExit"), true, true, true, true);

			HandleReordableList(b.stateRef.onFixedList, "On Fixed");
			HandleReordableList(b.stateRef.onUpdateList, "On Update");
			HandleReordableList(b.stateRef.onEnterList, "On Enter");
			HandleReordableList(b.stateRef.onExitList, "On Exit");
		}

        void HandleReordableList(ReorderableList list, string targetName)
        {
            list.drawHeaderCallback = (Rect rect) =>
            {
                EditorGUI.LabelField(rect, targetName);
            };

            list.drawElementCallback = (Rect rect, int index,bool isActive, bool isFocused) =>
             {
                 var element = list.serializedProperty.GetArrayElementAtIndex(index);
                 EditorGUI.ObjectField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), element, GUIContent.none);
             };
        }

        public override void DrawCurve(Node b)
        {

        }

        public void ClearReferences()
        {
      //      BehaviorEditor.ClearWindowsFromList(dependencies);
        //    dependencies.Clear();
        }

    }
}
