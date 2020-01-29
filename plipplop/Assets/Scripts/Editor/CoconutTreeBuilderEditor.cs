using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Rendering;

[CustomEditor(typeof(CoconutTreeBuilder))]
public class CoconutTreeBuilderEditor : Editor
{
    override public void  OnInspectorGUI () {
        CoconutTreeBuilder coconutTreeBuilder = (CoconutTreeBuilder)target;
        GUILayout.BeginHorizontal();
        if(GUILayout.Button("Generate Tree")) {
            Undo.RecordObject(target, "Generate new tree");
            coconutTreeBuilder.CreateTree();
        }
        if(GUILayout.Button("Save Tree")) {
            coconutTreeBuilder.SaveTree();
        }
        GUILayout.EndHorizontal();
        DrawDefaultInspector();
    }
}
