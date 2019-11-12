using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(Sound))]
public class SoundDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        // Don't make child fields be indented
        var indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        // Calculate rects
        var nameWidth = 0.5f;
        var linkWidth = 0.3f;
        var boolTagWidth = 0.15f;
        var boolWidth = 0.05f;
        var spaceBetween = 5f;
        var w = position.width - spaceBetween * 4f;

        var amountRect = new Rect(position.x, position.y, nameWidth * w, position.height);
        var unitRect = new Rect(amountRect.x + amountRect.width + spaceBetween, position.y, linkWidth * w, position.height);
        var nameRect = new Rect(unitRect.x + unitRect.width + spaceBetween, position.y, boolTagWidth* w, position.height);
        var boolRect = new Rect(nameRect.x + nameRect.width + spaceBetween, position.y, boolWidth * w, position.height);

        // Draw fields - passs GUIContent.none to each so they are drawn without labels
        EditorGUI.PropertyField(amountRect, property.FindPropertyRelative("name"), GUIContent.none);
        EditorGUI.PropertyField(unitRect, property.FindPropertyRelative("clip"), GUIContent.none);
        EditorGUI.LabelField(nameRect, "Loop?");
        EditorGUI.PropertyField(boolRect, property.FindPropertyRelative("loop"), GUIContent.none);

        // Set indent back to what it was
        EditorGUI.indentLevel = indent;

        EditorGUI.EndProperty();
    }
}