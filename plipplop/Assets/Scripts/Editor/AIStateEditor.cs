using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;
using UnityEditorInternal;

namespace Behavior.Editor
{
	 // [CustomEditor(typeof(AIState))]
	public class AIStateEditor : UnityEditor.Editor
    {
		SerializedObject serializedAIState;
		ReorderableList onFixedList;
		ReorderableList onUpdateList;
		ReorderableList onEnterList;
		ReorderableList onExitList;
		ReorderableList Transitions;

		bool showDefaultGUI = false;
		bool showActions = true;
		bool showTransitions = true;

		private void OnEnable()
        {
            serializedAIState = null;
		}

		public override void OnInspectorGUI()
		{
			showDefaultGUI = EditorGUILayout.Toggle("DefaultGUI", showDefaultGUI);
			if (showDefaultGUI)
			{
				base.OnInspectorGUI();
				return;
			}

			showActions = EditorGUILayout.Toggle("Show Actions", showActions);

			if(serializedAIState == null)
				SetupReordableLists();

			serializedAIState.Update();

			if (showActions)
			{	
				EditorGUILayout.LabelField("Actions that execute on FixedUpdate()");
				onFixedList.DoLayoutList();
				EditorGUILayout.LabelField("Actions that execute on Update()");
				onUpdateList.DoLayoutList();
				EditorGUILayout.LabelField("Actions that execute when entering this AIState");
				onEnterList.DoLayoutList();
				EditorGUILayout.LabelField("Actions that execute when exiting this AIState");
				onExitList.DoLayoutList();	
			}

			showTransitions = EditorGUILayout.Toggle("Show Transitions", showTransitions);

			if (showTransitions)
			{
				EditorGUILayout.LabelField("Conditions to exit this AIState");
				Transitions.DoLayoutList();
			}

			serializedAIState.ApplyModifiedProperties();
		}

		void SetupReordableLists()
		{
			AIState curAIState = (AIState)target;
			serializedAIState = new SerializedObject(curAIState);
			onFixedList = new ReorderableList(serializedAIState,serializedAIState.FindProperty("onFixed"), true, true, true, true);
			onUpdateList = new ReorderableList(serializedAIState,serializedAIState.FindProperty("onUpdate"), true, true, true, true);
			onEnterList = new ReorderableList(serializedAIState,serializedAIState.FindProperty("onEnter"), true, true, true, true);
			onExitList = new ReorderableList(serializedAIState,serializedAIState.FindProperty("onExit"), true, true, true, true);
			Transitions = new ReorderableList(serializedAIState, serializedAIState.FindProperty("transitions"), true, true, true, true);

			HandleReordableList(onFixedList, "On Fixed");
			HandleReordableList(onUpdateList, "On Update");
			HandleReordableList(onEnterList, "On Enter");
			HandleReordableList(onExitList, "On Exit");
			HandleTransitionReordable(Transitions, "Condition --> New AIState");
		}

		void HandleReordableList(ReorderableList list, string targetName)
		{
			list.drawHeaderCallback = (Rect rect) =>
			{
				EditorGUI.LabelField(rect, targetName);
			};

			list.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
			{
				var element = list.serializedProperty.GetArrayElementAtIndex(index);
				EditorGUI.ObjectField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), element, GUIContent.none);
			};
		}

		void HandleTransitionReordable(ReorderableList list, string targetName)
		{
			list.drawHeaderCallback = (Rect rect) =>
			{
				EditorGUI.LabelField(rect, targetName);
			};

			list.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
			{
				var element = list.serializedProperty.GetArrayElementAtIndex(index);
				//EditorGUI.ObjectField(new Rect(rect.x, rect.y, rect.width * .3f, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("conditions"), GUIContent.none);
				EditorGUI.ObjectField(new Rect(rect.x + + (rect.width *.35f), rect.y, rect.width * .3f, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("targetAIState"), GUIContent.none);
				EditorGUI.LabelField(new Rect(rect.x + +(rect.width * .75f), rect.y, rect.width * .2f, EditorGUIUtility.singleLineHeight), "Disable");
				EditorGUI.PropertyField(new Rect(rect.x + +(rect.width * .90f), rect.y, rect.width * .2f, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("disable"), GUIContent.none);

			};
		}

	}
}
