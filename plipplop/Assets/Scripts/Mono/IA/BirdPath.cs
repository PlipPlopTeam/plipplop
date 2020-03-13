using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif


[System.Serializable]
public class BirdPath : MonoBehaviour
{
	[System.Serializable]
	public class Point
	{
		public Vector3 position;
		public float range;
	}

	public bool loop = true;
    public List<Point> points = new List<Point>();

	private void Awake()
	{
		Game.i.aiZone.Register(this);
	}

	public Vector3 GetPosition(int id)
	{
		return points[id].position + Geometry.GetRandomPointInSphere(points[id].range);
	}

#if UNITY_EDITOR
	void OnDrawGizmosSelected()
	{
		for (int i = 0; i < points.Count; i++)
		{
			Gizmos.DrawWireSphere(points[i].position, points[i].range);
		}
	}
#endif
}

#if UNITY_EDITOR
[CustomEditor(typeof(BirdPath)), CanEditMultipleObjects]
[ExecuteInEditMode]
public class BirdPathEditor : Editor
{
	public override void OnInspectorGUI()
	{
		EditorGUI.BeginChangeCheck();

		BirdPath path = (BirdPath)target;
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
			BirdPath.Point p = new BirdPath.Point();
			if(path.points.Count != 0)
			{
				p.position = path.points[path.points.Count - 1].position;
			}
			p.range = 2f;
			path.points.Add(p);
		}

		if (EditorGUI.EndChangeCheck()) EditorUtility.SetDirty(path);
	}

	private void OnSceneGUI()
    {
        EditorGUI.BeginChangeCheck();

        BirdPath path = (BirdPath)target;
		BirdPath.Point[] points = path.points.ToArray();

        for (int i = 0; i < points.Length; i++)
        {
			points[i].position = Handles.PositionHandle(points[i].position, Quaternion.identity);
			GUI.skin.label.normal.textColor = Color.white;
            Handles.Label(points[i].position + Camera.current.transform.right * 0.5f, "#"+i.ToString(), GUI.skin.label);

            if(i < points.Length-1) DrawIntersect(points[i], points[i + 1]);
            else if(i == points.Length-1) if (path.loop) DrawIntersect(points[i], points[0]);
        }

        if(EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(path, "Move Zone Path Point");
            path.points = new List<BirdPath.Point>(points);
            EditorUtility.SetDirty(path);
        }
    }

	void DrawIntersect(BirdPath.Point from, BirdPath.Point to)
	{	
		float distance = Vector3.Distance(from.position, to.position);
		if (distance < from.range || distance < to.range) return;
		Vector3 direction = (from.position - to.position).normalized;
		Handles.DrawDottedLine(from.position + (from.range * -direction), to.position + (to.range * direction), 10f);
	}
}
#endif
