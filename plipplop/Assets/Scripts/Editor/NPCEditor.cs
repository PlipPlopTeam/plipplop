using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(NPC)), CanEditMultipleObjects]
public class NPCEditor : Editor
{
    private void OnSceneGUI()
    {
        EditorGUI.BeginChangeCheck();

        NPC npc = (NPC)target;

        Vector3[] newPath = npc.path;
        for(int i = 0; i < newPath.Length; i++)
        {
            newPath[i] = Handles.PositionHandle(newPath[i], Quaternion.identity);
        }

        if(EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(npc, "Move NPC path point");
            npc.path = newPath;
        }
    }
}
