using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(Sounds))]
public class SoundsDrawer : PropertyDrawer
{
    float addButtonHeight = 15f;
    float lineHeight = 20f;
    float interline = 4f;
    float spacingBeforeButton = 2f;
    float margin = 16f;
    int children = 0;

    public override void OnGUI(Rect pos, SerializedProperty prop, GUIContent label)
    {
        EditorGUI.BeginProperty(pos, new GUIContent(prop.displayName), prop);
        pos = EditorGUI.PrefixLabel(pos, GUIContent.none);
        pos.height = GetPropertyHeight(prop, label) - margin;
        pos.y += margin / 2f;

        prop.Next(true);
        prop.Next(true);

        var array = prop.Copy();
        children = array.arraySize;

        for (int i = 0; i < array.arraySize; i++) {
            SerializedProperty sound = array.GetArrayElementAtIndex(i);

            var thisPos = new Rect(pos);
            thisPos.width -= 20f;
            thisPos.height = lineHeight;
            thisPos.y = pos.y + i * lineHeight + i * interline;
            EditorGUI.PropertyField(thisPos, sound);

            //Remove this index from the List
            var minusRect = new Rect(thisPos);
            minusRect.x = thisPos.x + thisPos.width;
            minusRect.width = 20f;
            if (GUI.Button(minusRect, "-")) {
                array.DeleteArrayElementAtIndex(i);
            }
        }

        var buttonRect = new Rect(pos);
        buttonRect.height = addButtonHeight;
        buttonRect.y = pos.y + pos.height - buttonRect.height + spacingBeforeButton/2f;
        if (GUI.Button(buttonRect, "Add sound")) {
            array.InsertArrayElementAtIndex(array.arraySize);
        }
        
        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return lineHeight * children + interline  * (children-1) + spacingBeforeButton + addButtonHeight + margin;
    }
}