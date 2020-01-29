using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
#endif
using System;
using System.IO;

namespace Behavior.Editor
{
    public class AIStateDrawNode : DrawNode
    {
		#if UNITY_EDITOR
		public override void DrawWindow(Node node)
        {
            if (!(node is AIStateNode))
			{
                Debug.LogError("Attempted to draw a non-AIstate node with a AIStateDrawNode");
			}

			var b = (AIStateNode)node;

			

			if (b.IsStartNode()) {
                EditorGUILayout.LabelField("Start node. The behavior will begin from here.");
                try {
                    EditorGUILayout.LabelField("Initial state is id:" + BehaviorEditor.currentGraph.initialStateID);
                }
                catch { };
                return;
            }
            if(b.state == null)
            {
                EditorGUILayout.LabelField("Add state to modify:");
            }
            else
            {
				node.windowTitle = "State - " + b.state.name;
				string collapseText = "";
                if(!b.collapse)
                {
                    collapseText = "Collapse";
                }
                else
                {
                    collapseText = "Expand";
                    b.optimalHeight = 65;
                }

                b.collapse = EditorGUILayout.Toggle(collapseText, b.collapse);
            }

            b.state = (AIState)EditorGUILayout.ObjectField(b.state, typeof(AIState), false);

            if(b.previousCollapse != b.collapse)
            {
                b.previousCollapse = b.collapse;
            }

            if (b.state != null)
            {
				float standard = 215;

				if (!b.collapse)
                {
					SerializedObject sState = new SerializedObject(b.state);
					EditorGUILayout.PropertyField(sState.FindProperty("onFixed"), true);
					EditorGUILayout.PropertyField(sState.FindProperty("onUpdate"), true);
					EditorGUILayout.PropertyField(sState.FindProperty("onEnter"), true);
					EditorGUILayout.PropertyField(sState.FindProperty("onExit"), true);
					standard += (b.state.onFixed.Length + b.state.onUpdate.Length + b.state.onEnter.Length + b.state.onExit.Length) * 50;
					/*
					if (b.serializedAIState == null)
					{
						SetupReordableLists(b);

					//	SerializedObject serializedAIState = new SerializedObject(b.currentAIState);
					}
					*/
					//b.serializedAIState.Update();
					//b.showActions = EditorGUILayout.Toggle("Show Actions ", b.showActions);
					//b.serializedAIState.ApplyModifiedProperties();
                    b.optimalHeight = standard;
                }   
            }
		}

		void SetupReordableLists(AIStateNode b)
		{
			/*
			//b.serializedAIState = new SerializedObject(b.currentAIState);
			b.onFixedList = new ReorderableList(b.serializedAIState, b.serializedAIState.FindProperty("onFixed"), true, true, true, true);
			b.onUpdateList = new ReorderableList(b.serializedAIState, b.serializedAIState.FindProperty("onUpdate"), true, true, true, true);
			b.onEnterList = new ReorderableList(b.serializedAIState, b.serializedAIState.FindProperty("onEnter"), true, true, true, true);
			b.onExitList = new ReorderableList(b.serializedAIState, b.serializedAIState.FindProperty("onExit"), true, true, true, true);

			HandleReordableList(b.onFixedList, "On Fixed");
			HandleReordableList(b.onUpdateList, "On Update");
			HandleReordableList(b.onEnterList, "On Enter");
			HandleReordableList(b.onExitList, "On Exit");
			*/
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
		#endif
	}
}
