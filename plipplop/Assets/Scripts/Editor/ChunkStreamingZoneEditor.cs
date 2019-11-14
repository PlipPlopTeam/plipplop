using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ChunkStreamingZone)), CanEditMultipleObjects]
[ExecuteInEditMode]
public class ChunkStreamingZoneEditor : BaseEditor
{
    private void OnSceneGUI()
    {
        EditorGUI.BeginChangeCheck();

        ChunkStreamingZone chunkVol = (ChunkStreamingZone)target;
        Handles.matrix = chunkVol.transform.localToWorldMatrix;

        Vector3[] points = chunkVol.positions.ToArray();

        // Angle safety check
        for (int i = 0; i < points.Length; i++) {
            var previousPoint = i == 0 ? points[points.Length - 1] : points[i - 1];
            var nextPoint = i == points.Length - 1 ? points[0] : points[i + 1];
            var a = previousPoint - points[i];
            var b = nextPoint - points[i];

            var angle = Vector2.SignedAngle(new Vector2(a.x, a.z), new Vector2(b.x, b.z));
            var wrongAngle = angle < 0;
            if (wrongAngle) {
                var color = Color.Lerp(Color.red, Color.white, Mathf.Sin(Time.time * 10f));
                chunkVol.Draw(color, color);
            }
        }

        // Handles
        for (int i = 0; i < points.Length; i++) {
            var op = points[i];
            points[i] = Handles.FreeMoveHandle(points[i], Quaternion.Euler(Vector3.up), 3f, Vector3.zero, Handles.ConeHandleCap);
            points[i].y = 0f;

            // Angle limit
            var previousPoint = i == 0 ? points[points.Length - 1] : points[i - 1];
            var nextPoint = i == points.Length - 1 ? points[0] : points[i + 1];
            var a = previousPoint - points[i];
            var b = nextPoint - points[i];

            var angle = Vector2.SignedAngle(new Vector2(a.x, a.z), new Vector2(b.x, b.z));
            var wrongAngle = angle < 0;

            if (angle < 0) {
                points[i] = op;
                points[i].y = 0f;
            }

            // Snapping
            foreach (var neighbor in chunkVol.neighbors) {
                if (neighbor == null) continue;
                foreach (var nearPoint in neighbor.positions) {
                    var np = neighbor.transform.TransformPoint(nearPoint);
                    var wp = chunkVol.transform.TransformPoint(points[i]);
                    if (Vector3.Distance(np, wp) < 4f) {
                        points[i] = chunkVol.transform.InverseTransformPoint(np);
                    }
                }
            }
        }

        if (EditorGUI.EndChangeCheck()) {
            Undo.RecordObject(chunkVol, "Move chunk point");
            chunkVol.positions = new List<Vector3>(points);
            EditorUtility.SetDirty(chunkVol);
        }
    }

    public override void OnInspectorGUI()
    {
        // Chunk 



        DrawDefaultInspector();
    }
}
