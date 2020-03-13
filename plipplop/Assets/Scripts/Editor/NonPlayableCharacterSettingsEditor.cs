using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.Experimental.Rendering.HDPipeline;

[ExecuteInEditMode]
[CustomEditor(typeof(NonPlayableCharacterSettings)), CanEditMultipleObjects]
public class NonPlayableCharacterSettingsEditor : Editor
{
	public void OnValidate()
	{
		NonPlayableCharacterSettings settings = (NonPlayableCharacterSettings)target;
		Preview(settings);
	}

	public override void OnInspectorGUI()
	{
		NonPlayableCharacterSettings settings = (NonPlayableCharacterSettings)target;
		DrawDefaultInspector();
		//if (GUILayout.Button("Preview")) Preview(settings);
	}

	public void Preview(NonPlayableCharacterSettings s)
	{
		/*
		var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
		EditorSceneManager.SetActiveScene(scene);
		Stage();
		GameObject o = PrefabUtility.InstantiatePrefab(s.prefab) as GameObject;
		NonPlayableCharacter npc = o.GetComponent<NonPlayableCharacter>();
		npc.Awake();
		npc.Set(s);
		npc.EquipOutfit();
		*/
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