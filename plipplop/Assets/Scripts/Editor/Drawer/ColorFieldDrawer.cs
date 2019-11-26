using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(ColorFieldAttribute))]
public class ColorFieldDrawer : PropertyDrawer
{
    // Necessary since some properties tend to collapse smaller than their content
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property, label, true);
    }

    // Draw a disabled property field when ingame
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var oc = GUI.color;
        GUI.color = Color.Lerp(Color.white, ((ColorFieldAttribute)this.attribute).c, 0.75f);
        EditorGUI.PropertyField(position, property, label, true);
        GUI.color = oc;
    }
}