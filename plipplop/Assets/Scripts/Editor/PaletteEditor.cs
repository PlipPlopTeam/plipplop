using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Palette)), CanEditMultipleObjects]
public class PaletteEditor : Editor
{
	Vector2 scroll;

	public override void OnInspectorGUI()
	{
		GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.MaxWidth(40.0f) };
		SerializedProperty colors = serializedObject.FindProperty("colors");

		// Adding color row
		EditorGUILayout.BeginHorizontal();
		if (GUILayout.Button("Add Row")) AddRow();
		EditorGUILayout.EndHorizontal();

		scroll = EditorGUILayout.BeginScrollView(scroll);
		for (int i = 0; i < colors.arraySize; i++)
		{
			EditorGUILayout.BeginHorizontal();

			if (GUILayout.Button("Remove Row")) RemoveRow(i);
			if (i >= colors.arraySize) continue;

			var list = colors.GetArrayElementAtIndex(i);
			list.NextVisible(true);
			for (int j = 0; j < list.arraySize; j++)
			{
				var color = list.GetArrayElementAtIndex(j);
				color.colorValue = EditorGUILayout.ColorField(new GUIContent(), color.colorValue, false, true, false, options);
			}
			EditorGUILayout.EndHorizontal();
		}
		EditorGUILayout.EndScrollView();

		// Adding color columns
		EditorGUILayout.BeginHorizontal();
		if (GUILayout.Button("Add Color Element")) AddElement();
		if (GUILayout.Button("Remove Color Element")) RemoveElement();
		EditorGUILayout.EndHorizontal();

		// Clearing
		if (GUILayout.Button("Clear"))
		{
			colors.ClearArray();
		}

		serializedObject.ApplyModifiedProperties();

		void AddRow()
		{
			colors.InsertArrayElementAtIndex(colors.arraySize);

			var list = colors.GetArrayElementAtIndex(colors.arraySize - 1);
			list.NextVisible(true);
			if (list.arraySize == 0)
			{
				list.InsertArrayElementAtIndex(list.arraySize);
				list.GetArrayElementAtIndex(list.arraySize - 1).colorValue = RandColor();
			}

		}
		void RemoveRow(int at)
		{
			if (at == colors.arraySize) at = colors.arraySize - 1;
			colors.DeleteArrayElementAtIndex(at);
		}
		void AddElement()
		{
			for (int i = 0; i < colors.arraySize; i++)
			{
				var list = colors.GetArrayElementAtIndex(i);
				list.NextVisible(true);
				list.InsertArrayElementAtIndex(list.arraySize);
				list.GetArrayElementAtIndex(list.arraySize - 1).colorValue = RandColor();
			}
		}
		void RemoveElement()
		{
			for (int i = 0; i < colors.arraySize; i++)
			{
				var list = colors.GetArrayElementAtIndex(i);
				list.NextVisible(true);
				list.DeleteArrayElementAtIndex(list.arraySize - 1);
			}
		}
	}

	Color RandColor()
	{
		int rand = Random.Range(0, 3);
		if (rand == 0) return Color.red;
		else if (rand == 1) return Color.yellow;
		else if (rand == 2) return Color.blue;
		return Color.white;
	}
}
