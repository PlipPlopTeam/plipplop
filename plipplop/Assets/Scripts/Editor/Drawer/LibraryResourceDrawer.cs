using UnityEngine;
using UnityEditor;
using Behavior;

[CustomPropertyDrawer(typeof(NpcLibrary.AIStateResource))]
[CustomPropertyDrawer(typeof(NpcLibrary.AIConditionResource))] 
[CustomPropertyDrawer(typeof(NpcLibrary.AIActionResource))] 
public class LibraryResourceDrawer : PropertyDrawer
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