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


	public override void OnInspectorGUI()
	{
		EditorGUI.BeginChangeCheck();

		AIPath path = (AIPath)target;
		path.loop = EditorGUILayout.Toggle("Loop", path.loop);

		for (int i = 0; i < path.points.Count; i++)
		{
			EditorGUILayout.BeginHorizontal();
			if (GUILayout.Button("Remove")) path.points.RemoveAt(i);
			path.points[i].position = EditorGUILayout.Vector3Field("", path.points[i].position);
			path.points[i].range = EditorGUILayout.FloatField("", path.points[i].range);
			EditorGUILayout.EndHorizontal();
		}

		if (GUILayout.Button("Add"))
		{
			AIPath.Point p = new AIPath.Point();
			if(path.points.Count != 0)
			{
				p.position = path.points[path.points.Count - 1].position;
			}
			p.range = 2f;
			path.points.Add(p);
		}

		if (EditorGUI.EndChangeCheck())
		{
			EditorUtility.SetDirty(path);
		}
	}

	private void OnSceneGUI()
    {
        EditorGUI.BeginChangeCheck();

        AIPath path = (AIPath)target;
		AIPath.Point[] points = path.points.ToArray();

        for (int i = 0; i < points.Length; i++)
        {
			Handles.DrawWireDisc(points[i].position, Vector3.up, points[i].range);

            points[i].position = 
                Vector3.Scale(Handles.FreeMoveHandle(points[i].position, Quaternion.identity, 1f, Vector3.zero, GroundCap), Vector3.right + Vector3.forward) + 
                Vector3.Scale(Handles.FreeMoveHandle(points[i].position, Quaternion.identity, 1f, Vector3.zero, VerticalCap), Vector3.up)
            ;

			GUI.skin.label.normal.textColor = Color.white;
            Handles.Label(points[i].position + Camera.current.transform.right * 0.5f, "#"+i.ToString(), GUI.skin.label);

            if(i < points.Length-1)
            {
				DrawIntersect(points[i], points[i + 1]);
			}
            else if(i == points.Length-1)
            {
                if(path.loop) DrawIntersect(points[i], points[0]);
            }
        }

        if(EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(path, "Move Zone Path Point");
            path.points = new List<AIPath.Point>(points);
            EditorUtility.SetDirty(path);
        }
    }

	void DrawIntersect(AIPath.Point from, AIPath.Point to)
	{	
		float distance = Vector3.Distance(from.position, to.position);
		Vector3 direction = (from.position - to.position).normalized;

		Handles.DrawDottedLine(from.position + (from.range * -direction), to.position + (to.range * direction), 10f);
	}
}
