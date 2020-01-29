using UnityEngine;
using UnityEditor;
using Behavior;

[CustomEditor(typeof(NpcLibrary)), CanEditMultipleObjects]
[ExecuteInEditMode]
public class NpcLibraryEditor : Editor
{
	public override void OnInspectorGUI()
	{
		GUIStyle title = new GUIStyle();
		title.fontSize = 52;
		title.fontStyle = FontStyle.Bold;
		title.normal.textColor = Color.grey;
		title.alignment = TextAnchor.UpperCenter;

		SerializedProperty settingProperty = serializedObject.FindProperty("defaultSettings");
		SerializedProperty statesProperty = serializedObject.FindProperty("states");
		SerializedProperty conditionsProperty = serializedObject.FindProperty("conditions");
		SerializedProperty actionsProperty = serializedObject.FindProperty("actions");

		settingProperty.objectReferenceValue = EditorGUILayout.ObjectField("Default Settings", settingProperty.objectReferenceValue, typeof(NonPlayableCharacterSettings), true);
		
		// STATES
		EditorGUILayout.LabelField("STATE", title);
		for (int i = 0; i < statesProperty.arraySize; i++)
		{
			EditorGUILayout.BeginHorizontal();
			var prop = statesProperty.GetArrayElementAtIndex(i);
			prop.NextVisible(true);
			prop.intValue = EditorGUILayout.IntField(prop.intValue);
			prop.NextVisible(true);
			prop.objectReferenceValue = EditorGUILayout.ObjectField(prop.objectReferenceValue, typeof(AIState), true);
			if (GUILayout.Button("Delete")) statesProperty.DeleteArrayElementAtIndex(i);
			EditorGUILayout.EndHorizontal();

		}
		if (GUILayout.Button("Add"))
		{
			AIStateResource r = new AIStateResource();
			statesProperty.InsertArrayElementAtIndex(statesProperty.arraySize);
			var prop = statesProperty.GetArrayElementAtIndex(statesProperty.arraySize-1);
			prop.NextVisible(true);
			prop.intValue = statesProperty.arraySize;
			prop.NextVisible(true);
			prop.objectReferenceValue = null;
		}

		// CONDITIONS
		EditorGUILayout.LabelField("CONDITIONS", title);
		for (int i = 0; i < conditionsProperty.arraySize; i++)
		{
			EditorGUILayout.BeginHorizontal();
			var prop = conditionsProperty.GetArrayElementAtIndex(i);
			prop.NextVisible(true);
			prop.intValue = EditorGUILayout.IntField(prop.intValue);
			prop.NextVisible(true);
			prop.objectReferenceValue = EditorGUILayout.ObjectField(prop.objectReferenceValue, typeof(Condition), true);
			if (GUILayout.Button("Delete")) conditionsProperty.DeleteArrayElementAtIndex(i);
			EditorGUILayout.EndHorizontal();
		}
		if (GUILayout.Button("Add"))
		{
			AIStateResource r = new AIStateResource();
			conditionsProperty.InsertArrayElementAtIndex(conditionsProperty.arraySize);
			var prop = conditionsProperty.GetArrayElementAtIndex(conditionsProperty.arraySize - 1);
			prop.NextVisible(true);
			prop.intValue = conditionsProperty.arraySize;
			prop.NextVisible(true);
			prop.objectReferenceValue = null;
		}

		// ACTIONS
		EditorGUILayout.LabelField("ACTIONS", title);
		for (int i = 0; i < actionsProperty.arraySize; i++)
		{
			EditorGUILayout.BeginHorizontal();
			var prop = actionsProperty.GetArrayElementAtIndex(i);
			prop.NextVisible(true);
			prop.intValue = EditorGUILayout.IntField(prop.intValue);
			prop.NextVisible(true);
			prop.objectReferenceValue = EditorGUILayout.ObjectField(prop.objectReferenceValue, typeof(AIAction), true);
			if (GUILayout.Button("Delete")) actionsProperty.DeleteArrayElementAtIndex(i);
			EditorGUILayout.EndHorizontal();
		}
		if (GUILayout.Button("Add"))
		{
			AIStateResource r = new AIStateResource();
			actionsProperty.InsertArrayElementAtIndex(actionsProperty.arraySize);
			var prop = actionsProperty.GetArrayElementAtIndex(actionsProperty.arraySize - 1);
			prop.NextVisible(true);
			prop.intValue = actionsProperty.arraySize;
			prop.NextVisible(true);
			prop.objectReferenceValue = null;
		}

		// Reset Resource IDs
		if (GUILayout.Button("Reset"))
		{
			int counter = 0;

			for (int i = 0; i < statesProperty.arraySize; i++)
			{
				var prop = statesProperty.GetArrayElementAtIndex(i);
				prop.NextVisible(true);
				prop.intValue = counter++;
			}
			counter = 0;
			for (int i = 0; i < conditionsProperty.arraySize; i++)
			{
				var prop = conditionsProperty.GetArrayElementAtIndex(i);
				prop.NextVisible(true);
				prop.intValue = counter++;
			}
			counter = 0;
			for (int i = 0; i < actionsProperty.arraySize; i++)
			{
				var prop = actionsProperty.GetArrayElementAtIndex(i);
				prop.NextVisible(true);
				prop.intValue = counter++;
			}
		}
		
		serializedObject.ApplyModifiedProperties();
	}
}
