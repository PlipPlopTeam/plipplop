using UnityEngine;
using UnityEditor;
using Behavior.Editor;

[CustomEditor(typeof(NonPlayableCharacter)), CanEditMultipleObjects]
[ExecuteInEditMode]
public class NonPlayerCharacterEditor : Editor
{
	public override void OnInspectorGUI()
	{
		GUIStyle active = new GUIStyle(EditorStyles.toolbarButton);
		active.normal.textColor = Color.green;
		active.fontStyle = FontStyle.Bold;
		active.fontSize = 11;

		GUIStyle unactive = new GUIStyle(EditorStyles.toolbarButton);
		unactive.normal.textColor = Color.black;
		unactive.fontSize = 9;


		NonPlayableCharacter npc = (NonPlayableCharacter)target;
		DrawDefaultInspector();

		if (EditorApplication.isPlaying)
		{
			GUILayout.Label("States", EditorStyles.boldLabel);
			foreach(BehaviorGraphData.StateData sd in npc.graph.states)
			{
				if (npc.graph.IsCurrent(sd.id))
				{
					if (GUILayout.Button(Game.i.library.npcLibrary.GetAIStateObject(sd.stateId).name, active))
					{}
				}
				else
				{
					if (GUILayout.Button(Game.i.library.npcLibrary.GetAIStateObject(sd.stateId).name, unactive))
					{
						npc.graph.Move(sd.id);
					}
				}
			}
		}
	}
}