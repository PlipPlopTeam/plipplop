using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(AIStateResource))]
public class AIStateResourceDrawer : PropertyDrawer
{
	// Necessary since some properties tend to collapse smaller than their content
	public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
	{
		return EditorGUI.GetPropertyHeight(property);
	}

	// Draw a disabled property field when ingame
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		EditorGUILayout.BeginHorizontal();
		property.Next(true);
		EditorGUILayout.PropertyField(property, new GUIContent(), true);
		property.Next(false);
		EditorGUILayout.PropertyField(property, new GUIContent(), true);
		EditorGUILayout.EndHorizontal();
	}
}
