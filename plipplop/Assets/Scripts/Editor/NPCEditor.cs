﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(AgentMovement)), CanEditMultipleObjects]
[ExecuteInEditMode]
public class AgentMovementEditor : Editor
{
    private void OnSceneGUI()
    {
        EditorGUI.BeginChangeCheck();

        AgentMovement am = (AgentMovement)target;
        if(am.path == null) return;
        
        Vector3[] newPath = am.path.points;
        for(int i = 0; i < newPath.Length; i++)
        {
            newPath[i] = Handles.PositionHandle(newPath[i], Quaternion.identity);
        }

        if(EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(am, "Move NPC path point");
            am.path.points = newPath;
            EditorUtility.SetDirty(am);
        }
    }
}
