using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Behavior.Editor;
using Behavior;
using System.Linq;

[CustomEditor(typeof(BehaviorGraph)), CanEditMultipleObjects]
public class BehaviorGraphEditor : Editor
{
	//private int counter = 0;
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
			BehaviorGraphData graphData = CreateInstance<BehaviorGraphData>();
			AssetDatabase.CreateAsset(graphData, "Assets/BehaviorGraphData.asset");
			AssetDatabase.SaveAssets();
			EditorUtility.FocusProjectWindow();
			Selection.activeObject = graphData;
			Compile(graphData, graph);
			AssetDatabase.RenameAsset("Assets/BehaviorGraphData.asset", target.name + "_Exported");
		}
	}

	public void Compile(BehaviorGraphData graphData, BehaviorGraph graphObject)
	{
		NpcLibrary lib = (NpcLibrary)Resources.Load("Library");
		graphData.states.Clear();
		graphData.transitions.Clear();
		graphData.initialNode = graphObject.initialStateID;

		foreach (AIStateNode sn in graphObject.stateNodes)
		{
			if (sn.id == -1) continue;

			BehaviorGraphData.StateData sd = new BehaviorGraphData.StateData();
			sd.id = sn.id;
			if (sn.state != null) sd.name = sn.state.name;
			else sd.name = "Initial";
			sd.stateId = lib.GetAIStateId(sn.state);
			sd.hasExits = sn.exitNodes.Select(o => { return o.HasValue; }).ToArray();
			sd.exits = sn.exitNodes.Select(o => { return o.HasValue ? o.Value : 0; }).ToArray();
			graphData.states.Add(sd);
		}

		foreach (AIStateTransitionNode tdn in graphObject.transitionNodes)
		{
			BehaviorGraphData.TransitionData td = new BehaviorGraphData.TransitionData();
			td.id = tdn.id;
			List<int> conds = new List<int>();
			foreach (Condition c in tdn.conditions)
			{
				conds.Add(lib.GetConditionId(c));
			}
			td.conditions = conds.ToArray();
			td.hasExits = tdn.exitNodes.Select(o => { return o.HasValue; }).ToArray();
			td.exits = tdn.exitNodes.Select(o => { return o.HasValue ? o.Value : 0; }).ToArray();
			graphData.transitions.Add(td);
		}
	}
}
