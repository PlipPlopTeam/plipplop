using UnityEngine;
using UnityEditor;
using Behavior;

[CustomPropertyDrawer(typeof(NpcLibrary.AIStateResource))]
public class LibraryResourceDrawer : PropertyDrawer
{
    // Necessary since some properties tend to collapse smaller than their content
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return 20f;
    }

    // Draw a disabled property field when ingame
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        property.Next(true);
        EditorGUI.PropertyField(position.Shift(Vector2.left * 50f), property, new GUIContent("id:" + property.intValue.ToString()), true);
        property.Next(false);
        EditorGUI.PropertyField(position, property, new GUIContent(), true);
    }
}