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
    int size = 0;
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

            /*
            // Display the property fields in two ways.

            if (DisplayFieldType == 0) {// Choose to display automatic or custom field types. This is only for example to help display automatic and custom fields.
                //1. Automatic, No customization <-- Choose me I'm automatic and easy to setup
                EditorGUILayout.LabelField("Automatic Field By Property Type");
                EditorGUILayout.PropertyField(MyGO);
                EditorGUILayout.PropertyField(MyInt);
                EditorGUILayout.PropertyField(MyFloat);
                EditorGUILayout.PropertyField(MyVect3);

                // Array fields with remove at index
                EditorGUILayout.Space();
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Array Fields");

                if (GUILayout.Button("Add New Index", GUILayout.MaxWidth(130), GUILayout.MaxHeight(20))) {
                    MyArray.InsertArrayElementAtIndex(MyArray.arraySize);
                    MyArray.GetArrayElementAtIndex(MyArray.arraySize - 1).intValue = 0;
                }

                for (int a = 0; a < MyArray.arraySize; a++) {
                    EditorGUILayout.PropertyField(MyArray.GetArrayElementAtIndex(a));
                    if (GUILayout.Button("Remove  (" + a.ToString() + ")", GUILayout.MaxWidth(100), GUILayout.MaxHeight(15))) {
                        MyArray.DeleteArrayElementAtIndex(a);
                    }
                }
            }
            else {
                //Or

                //2 : Full custom GUI Layout <-- Choose me I can be fully customized with GUI options.
                EditorGUILayout.LabelField("Customizable Field With GUI");
                MyGO.objectReferenceValue = EditorGUILayout.ObjectField("My Custom Go", MyGO.objectReferenceValue, typeof(GameObject), true);
                MyInt.intValue = EditorGUILayout.IntField("My Custom Int", MyInt.intValue);
                MyFloat.floatValue = EditorGUILayout.FloatField("My Custom Float", MyFloat.floatValue);
                MyVect3.vector3Value = EditorGUILayout.Vector3Field("My Custom Vector 3", MyVect3.vector3Value);


                // Array fields with remove at index
                EditorGUILayout.Space();
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Array Fields");

                if (GUILayout.Button("Add New Index", GUILayout.MaxWidth(130), GUILayout.MaxHeight(20))) {
                    MyArray.InsertArrayElementAtIndex(MyArray.arraySize);
                    MyArray.GetArrayElementAtIndex(MyArray.arraySize - 1).intValue = 0;
                }

                for (int a = 0; a < MyArray.arraySize; a++) {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("My Custom Int (" + a.ToString() + ")", GUILayout.MaxWidth(120));
                    MyArray.GetArrayElementAtIndex(a).intValue = EditorGUILayout.IntField("", MyArray.GetArrayElementAtIndex(a).intValue, GUILayout.MaxWidth(100));
                    if (GUILayout.Button("-", GUILayout.MaxWidth(15), GUILayout.MaxHeight(15))) {
                        MyArray.DeleteArrayElementAtIndex(a);
                    }
                    EditorGUILayout.EndHorizontal();
                }
            }

        */
            EditorGUILayout.Space();

            //Remove this index from the List
            if (GUILayout.Button("-")) {
                mappingList.DeleteArrayElementAtIndex(i);
            }

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
        }
        EditorGUILayout.Space();

        //Apply the changes to our list
        targetList.ApplyModifiedProperties();
    }


    /*
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        var style = new GUIStyle();
        style.fixedWidth = 1000;
        style.fixedHeight = 300;

        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("+", EditorStyles.miniButton)) size++;
        if (GUILayout.Button("-", EditorStyles.miniButton)) size--;
        EditorGUILayout.EndHorizontal();

        for (var i = 0; i < size; i++) {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.EnumPopup(ACTION.ACTION);
            EditorGUILayout.EnumPopup(ACTION.ACTION);
            EditorGUILayout.EndHorizontal();
        }
    }
    */
}