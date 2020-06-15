using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(SpielbergClips.SwitchCamera))]
public class SpielbergClipDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        //base.OnGUI(position, property, label);
    }
}
