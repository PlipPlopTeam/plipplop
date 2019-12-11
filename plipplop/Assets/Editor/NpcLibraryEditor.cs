using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(NpcLibrary)), CanEditMultipleObjects]
[ExecuteInEditMode]
public class NpcLibraryEditor : Editor
{
	public override void OnInspectorGUI()
	{
		NpcLibrary lib = (NpcLibrary)target;

		DrawDefaultInspector();
		if (GUILayout.Button("Reset"))
		{
			int counter = 0;
			foreach(NpcLibrary.AIStateResource s in lib.states)
			{
				s.id = counter++;
			}
			counter = 0;
			foreach (NpcLibrary.AIConditionResource c in lib.conditions)
			{
				c.id = counter++;
			}
		}
	}
}
