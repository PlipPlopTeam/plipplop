using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

using System;
using System.IO;

namespace Behavior.Editor
{
    public class AIStateDrawNode : DrawNode
    {
        public override void DrawWindow(Node node)
        {
            if (!(node is AIStateNode)) {
                Debug.LogError("Attempted to draw a non-AIstate node with a AIStateDrawNode");
            }

            var b = (AIStateNode)node;
            if (b.IsStartNode()) {
                EditorGUILayout.LabelField("Start node. The behavior will begin from here.");
                try {
                    EditorGUILayout.LabelField("Initial state: " + b.currentAIState);
                }
                catch { };
                return;
            }
            if(b.currentAIState == null)
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

            b.currentAIState = (AIState)EditorGUILayout.ObjectField(b.currentAIState, typeof(AIState), false);

            if(b.previousCollapse != b.collapse)
            {
                b.previousCollapse = b.collapse;
            }

            if (b.currentAIState != null)
            {
                b.isAssigned = true;
                
                if (!b.collapse)
                {
					if (b.serializedAIState == null)
					{
						SetupReordableLists(b);

					//	SerializedObject serializedAIState = new SerializedObject(b.currentAIState);
					}

					float standard = 100;
					b.serializedAIState.Update();
					b.showActions = EditorGUILayout.Toggle("Show Actions ", b.showActions);
					if (b.showActions)
					{
						b.onFixedList.DoLayoutList();
						b.onUpdateList.DoLayoutList();
						standard += 125 + (b.onUpdateList.count + b.onFixedList.count) * 18;
					}
					b.showEnterExit = EditorGUILayout.Toggle("Show Enter/Exit ", b.showEnterExit);
					if (b.showEnterExit)
					{
						b.onEnterList.DoLayoutList();
						b.onExitList.DoLayoutList();
						standard += 125 + (b.onEnterList.count + b.onExitList.count) * 18;
					}
					b.serializedAIState.ApplyModifiedProperties();
                    b.optimalHeight = standard;
                }   
            }
            else
            {
                b.isAssigned = false;
            }
		}

		void SetupReordableLists(AIStateNode b)
		{

			b.serializedAIState = new SerializedObject(b.currentAIState);
			b.onFixedList = new ReorderableList(b.serializedAIState, b.serializedAIState.FindProperty("onFixed"), true, true, true, true);
			b.onUpdateList = new ReorderableList(b.serializedAIState, b.serializedAIState.FindProperty("onUpdate"), true, true, true, true);
			b.onEnterList = new ReorderableList(b.serializedAIState, b.serializedAIState.FindProperty("onEnter"), true, true, true, true);
			b.onExitList = new ReorderableList(b.serializedAIState, b.serializedAIState.FindProperty("onExit"), true, true, true, true);

			HandleReordableList(b.onFixedList, "On Fixed");
			HandleReordableList(b.onUpdateList, "On Update");
			HandleReordableList(b.onEnterList, "On Enter");
			HandleReordableList(b.onExitList, "On Exit");
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

        public void ClearReferences()
        {
      //      BehaviorEditor.ClearWindowsFromList(dependencies);
        //    dependencies.Clear();
        }

    }
}
