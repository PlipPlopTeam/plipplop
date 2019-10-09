using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static Mapping;

[CustomEditor(typeof(Mapping))]
public class MappingEditor : Editor
{
    SerializedObject targetList;
    SerializedProperty mappingList;
    Mapping list;

    void OnEnable()
    {
        list = (Mapping)target;
        targetList = new SerializedObject(list);
        mappingList = targetList.FindProperty("map"); // Find the List in our script and create a refrence of it
    }

    public override void OnInspectorGUI()
    {
        //Update our list
        targetList.Update();

        EditorGUILayout.Space();
        if (GUILayout.Button("Add")) {
            list.map.Add(new MappedAction());
        }
        EditorGUILayout.Space();


        //Display our list to the inspector window
        var width = Screen.width;

        for (int i = 0; i < mappingList.arraySize; i++) {
            SerializedProperty myListRef = mappingList.GetArrayElementAtIndex(i);

            EditorGUILayout.BeginHorizontal();

            SerializedProperty action = myListRef.FindPropertyRelative("action");
            SerializedProperty input = myListRef.FindPropertyRelative("input");
            SerializedProperty isInverted = myListRef.FindPropertyRelative("isInverted");
            SerializedProperty factor = myListRef.FindPropertyRelative("factor");
            

            EditorGUILayout.PropertyField(action, GUIContent.none, GUILayout.Width(width * 0.2f));
            EditorGUILayout.PropertyField(input, GUIContent.none, GUILayout.Width(width * 0.2f));
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Invert", GUILayout.Width(width * 0.1f));
            EditorGUILayout.PropertyField(isInverted, GUIContent.none, GUILayout.Width(width * 0.1f));
            EditorGUILayout.LabelField("x", GUILayout.Width(width * 0.015f));
            EditorGUILayout.PropertyField(factor, GUIContent.none, GUILayout.Width(width * 0.05f));

            EditorGUILayout.Space();

            //Remove this index from the List
            if (GUILayout.Button("-")) {
                mappingList.DeleteArrayElementAtIndex(i);
            }

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
        }
        EditorGUILayout.Space();

        targetList.ApplyModifiedProperties();
        EditorUtility.SetDirty(target);

    }

}