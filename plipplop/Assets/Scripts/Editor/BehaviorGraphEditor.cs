using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Behavior.Editor;

[CustomEditor(typeof(BehaviorGraph)), CanEditMultipleObjects]
public class BehaviorGraphEditor : Editor
{
	public override void OnInspectorGUI()
	{
		EditorGUI.BeginChangeCheck();

		BehaviorGraph graph = (BehaviorGraph)target;
		if (graph == null) return;

		EditorGUILayout.LabelField("DATAS");
		DrawDefaultInspector();
		EditorGUILayout.LabelField("FUNCTIONS");

		if (GUILayout.Button("EXPORT"))
		{
			BehaviorGraphData g = CreateInstance<BehaviorGraphData>();
			AssetDatabase.CreateAsset(g, "Assets/BehaviorGraphData.asset");
			AssetDatabase.SaveAssets();
			EditorUtility.FocusProjectWindow();
			Selection.activeObject = g;
			g.Compile(graph);
			g.name = target.name + "_Exported";
		}
	}
}

