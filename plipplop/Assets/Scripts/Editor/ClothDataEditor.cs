using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.Experimental.Rendering.HDPipeline;

[CustomEditor(typeof(ClothData)), CanEditMultipleObjects]
public class ClothDataEditor : Editor
{
	public override void OnInspectorGUI()
	{
		ClothData data = (ClothData)target;
		//DrawDefaultInspector();

		GUIStyle title = new GUIStyle();
		title.fontStyle = FontStyle.Bold;
		title.fontSize = 14;

		GUIStyle subtitle = new GUIStyle();
		subtitle.fontStyle = FontStyle.Bold;
		subtitle.fontSize = 11;

		GUIStyle button = new GUIStyle(EditorStyles.toolbarButton);
		button.normal.textColor = Color.black;
		button.fontStyle = FontStyle.Bold;
		button.fontSize = 12;

		GUIStyle active = new GUIStyle(EditorStyles.toolbarButton);
		active.normal.textColor = Color.grey;
		active.fontStyle = FontStyle.Bold;
		active.fontSize = 11;
		GUIStyle unactive = new GUIStyle(EditorStyles.toolbarButton);
		unactive.normal.textColor = Color.black;
		unactive.fontSize = 9;

		SerializedProperty name = serializedObject.FindProperty("title");
		SerializedProperty slot = serializedObject.FindProperty("slot");
		SerializedProperty prefab = serializedObject.FindProperty("prefab");
		SerializedProperty color = serializedObject.FindProperty("color");
		SerializedProperty bannedSlot = serializedObject.FindProperty("bannedSlot");

		if (GUILayout.Button("Preview", button))
		{
			var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
			EditorSceneManager.SetActiveScene(scene);
			PrefabUtility.InstantiatePrefab(Create(data), scene);
			Stage();
			Selection.activeObject = data;
		}

		EditorGUILayout.LabelField("Basics", title);
		EditorGUILayout.PropertyField(name);
		EditorGUILayout.PropertyField(prefab);
		EditorGUILayout.PropertyField(slot);

		EditorGUILayout.LabelField("Banned Slot", title);
		EditorGUILayout.HelpBox("Slots to unequip or banned if this cloth is equiped", MessageType.Info);
		for (int i = 0; i < bannedSlot.arraySize; i++)
		{
			EditorGUILayout.BeginHorizontal();
			if (GUILayout.Button("Remove", button))
			{
				int key = i;
				if (key == bannedSlot.arraySize) key = bannedSlot.arraySize - 1;
				bannedSlot.DeleteArrayElementAtIndex(key);
			}
			if (i >= bannedSlot.arraySize) continue;
			EditorGUILayout.PropertyField(bannedSlot.GetArrayElementAtIndex(i), new GUIContent(""));
			EditorGUILayout.EndHorizontal();
		}
		if (GUILayout.Button("Add", button)) bannedSlot.InsertArrayElementAtIndex(bannedSlot.arraySize);



		EditorGUILayout.LabelField("Color Modifier", title);
		color.NextVisible(true);
		EditorGUILayout.PropertyField(color);
		if (color.objectReferenceValue == null)
		{
			EditorGUILayout.HelpBox("Select a color palette to start adding color mods", MessageType.Warning);
		}
		else
		{
			color.NextVisible(true);
			EditorGUILayout.HelpBox("Write the names of the properties you want to apply the color modifier on.", MessageType.Info);
			for (int i = 0; i < color.arraySize; i++)
			{
				EditorGUILayout.BeginHorizontal();
				if (GUILayout.Button("Remove", button))
				{
					int key = i;
					if (key == color.arraySize) key = color.arraySize - 1;
					color.DeleteArrayElementAtIndex(key);
				}
				if (i >= color.arraySize) continue;
				EditorGUILayout.PropertyField(color.GetArrayElementAtIndex(i), new GUIContent(""));
				EditorGUILayout.EndHorizontal();
			}
			if (GUILayout.Button("Add", button)) color.InsertArrayElementAtIndex(color.arraySize);

			if(color.arraySize == 0)
			{
				EditorGUILayout.HelpBox("You need to specify some properties names, or the palette wont have any effect.", MessageType.Warning);
			}
		}
		serializedObject.ApplyModifiedProperties();
	}

	public GameObject Create(ClothData data)
	{
		GameObject obj = new GameObject();
		Cloth cloth = obj.AddComponent<Cloth>();
		obj.name = data.name;
		cloth.Create(data);
		return cloth.gameObject;
	}

	public void Stage()
	{
		Light l = new GameObject().AddComponent<Light>();
		l.type = LightType.Directional;
		l.transform.rotation = Quaternion.Euler(new Vector3(140f, 210f, 0f));
		l.gameObject.name = "Light";
	}

	public void Light(Vector3 pos)
	{
		GameObject o = new GameObject();
		o.transform.position = pos;
		Light l = new GameObject().AddComponent<Light>();
		HDAdditionalLightData hdl = o.GetComponent<HDAdditionalLightData>();
		hdl.intensity = 20f;
	}
}
