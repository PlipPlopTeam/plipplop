using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(Aperture.Range))]
public class ApertureRangeDrawer : PropertyDrawer
{
    public override void OnGUI(Rect pos, SerializedProperty prop, GUIContent label)
    {
        SerializedProperty min = prop.FindPropertyRelative("min");
        SerializedProperty max = prop.FindPropertyRelative("max");

        Rect labelRect = new Rect(pos);
        labelRect.width = pos.width / 1.5f;
        float remainingWidth = pos.width - labelRect.width;

        EditorGUIUtility.labelWidth = 100f;
        EditorGUI.BeginProperty(pos, new GUIContent(prop.displayName), prop);
        EditorGUI.LabelField(labelRect, label);

        var mX = labelRect.width + remainingWidth/5f;
        Rect minRect = new Rect(pos);
        minRect.x = mX - 15f;
        Rect minValueRect = new Rect(minRect);
        minValueRect.x = mX + 15f;
        minValueRect.width = 40f;

        var xX = labelRect.width + (remainingWidth*4f) / 5f;
        Rect maxRect = new Rect(pos);
        maxRect.x = xX - 15f;
        Rect maxValueRect = new Rect(maxRect);
        maxValueRect.x = xX + 15f;
        maxValueRect.width = 40f;


        EditorGUI.PrefixLabel(minRect, GUIUtility.GetControlID(FocusType.Passive), new GUIContent("Min"));
        min.floatValue = EditorGUI.FloatField(minValueRect, min.floatValue);
        EditorGUI.PrefixLabel(maxRect, GUIUtility.GetControlID(FocusType.Passive), new GUIContent("Max"));
        max.floatValue = EditorGUI.FloatField(maxValueRect, max.floatValue);

        prop.serializedObject.ApplyModifiedProperties();

        EditorGUI.EndProperty();
        /*
        pos.width = 30f;
        pos.x = minX;
        EditorGUI.PropertyField(pos, min);
        pos.x = maxX;
        EditorGUI.PropertyField(pos, max);
        */
    }
}