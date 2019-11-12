using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Zone)), CanEditMultipleObjects]
[ExecuteInEditMode]
public class ZoneEditor : Editor
{
    private void OnSceneGUI()
    {
        EditorGUI.BeginChangeCheck();

        Zone zone = (Zone)target;
        if(zone.paths.Length <= 0) return;
        
        AgentMovement.Path[] newPaths = zone.paths;

        foreach(AgentMovement.Path path in newPaths)
        {
            for(int i = 0; i < path.points.Length; i++)
            {
                path.points[i] = Handles.PositionHandle(path.points[i], Quaternion.identity);
                if(i < path.points.Length-1)
                {
                    Handles.DrawLine(path.points[i], path.points[i+1]);
                }
                else if(i == path.points.Length-1)
                {
                    if(path.loop) Handles.DrawLine(path.points[i], path.points[0]);
                }
            }
        }

        if(EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(zone, "Move Zone Path Point");
            zone.paths = newPaths;
            EditorUtility.SetDirty(zone);
        }
    }
}
