using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(AIPath)), CanEditMultipleObjects]
[ExecuteInEditMode]
public class AIPathEditor : Editor
{
    public void GroundCap(int controlID, Vector3 position, Quaternion rotation, float size, EventType eventType)
    {
        Handles.CircleHandleCap(controlID, position, Quaternion.LookRotation(Vector3.up), 0.35f, eventType);
    }
    public void VerticalCap(int controlID, Vector3 position, Quaternion rotation, float size, EventType eventType)
    {
        Handles.color = Color.green;
        Handles.ArrowHandleCap(controlID, position, Quaternion.LookRotation(Vector3.up), 1f, eventType);
        Handles.color = Color.white;
    }
    
    private void OnSceneGUI()
    {
        EditorGUI.BeginChangeCheck();

        AIPath path = (AIPath)target;
        var points = path.points.ToArray();

        for (int i = 0; i < points.Length; i++)
        {
            points[i] = 
                Vector3.Scale(Handles.FreeMoveHandle(points[i], Quaternion.identity, 1f, Vector3.zero, GroundCap), Vector3.right + Vector3.forward) + 
                Vector3.Scale(Handles.FreeMoveHandle(points[i], Quaternion.identity, 1f, Vector3.zero, VerticalCap), Vector3.up)
            ;

            Handles.Label(points[i] + Camera.current.transform.right, "#"+i.ToString(), GUI.skin.label);

            if(i < points.Length-1)
            {
                Handles.DrawDottedLine(points[i], points[i+1], 10f);
            }
            else if(i == points.Length-1)
            {
                if(path.loop) Handles.DrawDottedLine(points[i], points[0], 10f);
            }
        }

        if(EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(path, "Move Zone Path Point");
            path.points = new List<Vector3>(points);
            EditorUtility.SetDirty(path);
        }
    }
}
