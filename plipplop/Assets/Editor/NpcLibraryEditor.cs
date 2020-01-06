using UnityEngine;
using UnityEditor;
using Behavior;

[CustomEditor(typeof(NpcLibrary)), CanEditMultipleObjects]
[ExecuteInEditMode]
public class NpcLibraryEditor : Editor
{
	public override void OnInspectorGUI()
	{
		NpcLibrary lib = (NpcLibrary)target;
		//DrawDefaultInspector();

		GUIStyle title = new GUIStyle();
		title.fontSize = 52;
		title.fontStyle = FontStyle.Bold;
		title.normal.textColor = Color.grey;
		title.alignment = TextAnchor.UpperCenter;


		EditorGUILayout.LabelField("STATE", title);
		for (int i = 0; i < lib.states.Count; i++)
		{
			EditorGUILayout.BeginHorizontal();
			if (GUILayout.Button("Remove")) lib.states.RemoveAt(i);
			EditorGUILayout.IntField(lib.states[i].id);
			EditorGUILayout.ObjectField(lib.states[i].resource, typeof(AIState));
			EditorGUILayout.EndHorizontal();
		}
		if (GUILayout.Button("Add"))
		{
			lib.states.Add(new AIStateResource());
		}

		EditorGUILayout.LabelField("CONDITIONS", title);
		for (int i = 0; i < lib.conditions.Count; i++)
		{
			EditorGUILayout.BeginHorizontal();
			if (GUILayout.Button("Remove")) lib.conditions.RemoveAt(i);
			EditorGUILayout.IntField(lib.conditions[i].id);
			EditorGUILayout.ObjectField(lib.conditions[i].resource, typeof(AIState));
			EditorGUILayout.EndHorizontal();
		}
		if (GUILayout.Button("Add"))
		{
			lib.states.Add(new AIStateResource());
		}

		EditorGUILayout.LabelField("ACTIONS", title);
		for (int i = 0; i < lib.actions.Count; i++)
		{
			EditorGUILayout.BeginHorizontal();
			if (GUILayout.Button("Remove")) lib.actions.RemoveAt(i);
			EditorGUILayout.IntField(lib.actions[i].id);
			EditorGUILayout.ObjectField(lib.actions[i].resource, typeof(AIState));
			EditorGUILayout.EndHorizontal();
		}
		if (GUILayout.Button("Add"))
		{
			lib.states.Add(new AIStateResource());
		}


















		if (GUILayout.Button("Reset"))
		{
			int counter = 0;
			foreach(AIStateResource s in lib.states)
			{
				s.id = counter++;
			}
			counter = 0;
			foreach (AIConditionResource c in lib.conditions)
			{
				c.id = counter++;
			}
			counter = 0;
			foreach (AIActionResource a in lib.actions)
			{
				a.id = counter++;
			}
		}
	}
}
