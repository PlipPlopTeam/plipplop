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

		lib.defaultSettings = EditorGUILayout.ObjectField(lib.defaultSettings, typeof(NonPlayableCharacterSettings)) as NonPlayableCharacterSettings;

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
			lib.states[i].resource = EditorGUILayout.ObjectField(lib.states[i].resource, typeof(AIState)) as AIState;
			EditorGUILayout.EndHorizontal();
		}
		if (GUILayout.Button("Add"))
		{
			AIStateResource r = new AIStateResource();
			r.id = lib.states.Count;
			lib.states.Add(r);
		}

		EditorGUILayout.LabelField("CONDITIONS", title);
		for (int i = 0; i < lib.conditions.Count; i++)
		{
			EditorGUILayout.BeginHorizontal();
			if (GUILayout.Button("Remove")) lib.conditions.RemoveAt(i);
			EditorGUILayout.IntField(lib.conditions[i].id);
			lib.conditions[i].resource = EditorGUILayout.ObjectField(lib.conditions[i].resource, typeof(Condition)) as Condition;
			EditorGUILayout.EndHorizontal();
		}
		if (GUILayout.Button("Add"))
		{
			AIConditionResource r = new AIConditionResource();
			r.id = lib.conditions.Count;
			lib.conditions.Add(r);
		}

		EditorGUILayout.LabelField("ACTIONS", title);
		for (int i = 0; i < lib.actions.Count; i++)
		{
			EditorGUILayout.BeginHorizontal();
			if (GUILayout.Button("Remove")) lib.actions.RemoveAt(i);
			EditorGUILayout.IntField(lib.actions[i].id);
			lib.actions[i].resource = EditorGUILayout.ObjectField(lib.actions[i].resource, typeof(AIAction)) as AIAction;
			EditorGUILayout.EndHorizontal();
		}
		if (GUILayout.Button("Add"))
		{
			AIActionResource r = new AIActionResource();
			r.id = lib.actions.Count;
			lib.actions.Add(r);
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
