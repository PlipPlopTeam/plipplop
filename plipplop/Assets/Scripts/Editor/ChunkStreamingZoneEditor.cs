using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ChunkStreamingZone)), CanEditMultipleObjects]
[ExecuteInEditMode]
public class ChunkStreamingZoneEditor : BaseEditor
{
    readonly GUIStyle redStyle = GUIStyle.none;

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
        List<int> toRemove = new List<int>();
        for (int i = 0; i < points.Length; i++) {
            var op = points[i];
            Handles.color = Color.white;
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
            foreach (var neighbor in chunkVol.neighborhood) {
                if (neighbor == null) continue;
                foreach (var nearPoint in neighbor.positions) {
                    var np = neighbor.transform.TransformPoint(nearPoint);
                    var wp = chunkVol.transform.TransformPoint(points[i]);
                    if (Vector3.Distance(np, wp) < 4f) {
                        points[i] = chunkVol.transform.InverseTransformPoint(np);
                    }
                }
            }

            // Remove button
            Handles.color = Color.red;
            if (Handles.Button(
                points[i] + Camera.current.transform.right * 3f,
                Quaternion.LookRotation(Camera.current.transform.forward, Camera.current.transform.up),
                1f,
                1f,
                Handles.DotHandleCap
            )) {
                toRemove.Add(i);
            };
        }

        foreach(var index in toRemove) {
            chunkVol.positions.RemoveAt(index);
        }

        if (EditorGUI.EndChangeCheck()) {
            Undo.RecordObject(chunkVol, "Move chunk point");
            chunkVol.positions = new List<Vector3>(points);
            EditorUtility.SetDirty(chunkVol);
        }

        target.name = "ChunkZone_" + ((ChunkStreamingZone)target).identifier;
    }

    public override void OnInspectorGUI()
    {
        MakeStyles();

        EditorGUILayout.Space();

        IdentifierField()();
        EditorGUILayout.Space();

        ChunkSceneField()();
        EditorGUILayout.Space();

        PositionsField()();
        EditorGUILayout.Space();

        NeighborsField()();
        EditorGUILayout.Space();

    }

    System.Action IdentifierField()
    {
        var csz = (ChunkStreamingZone)target;

        var options = new List<GUILayoutOption>();

        options.Add(GUILayout.Height(fieldsHeight));

        var noBoldTitle = new GUIStyle(title);
        noBoldTitle.fontStyle = FontStyle.Normal;

        var icon = Resources.Load<Texture2D>("Editor/Sprites/SPR_D_ChunkID");

        return delegate {
            GUILayout.BeginHorizontal(options.ToArray());
            GUILayout.Label(new GUIContent(buttonSpace + "Identifier", icon), noBoldTitle, GUILayout.Height(fieldsHeight), GUILayout.Width(Screen.width * 0.3f));

            var id = serializedObject.FindProperty("identifier");
            id.stringValue = EditorGUILayout.TextField(id.stringValue, GUILayout.Height(fieldsHeight));
            if (id.stringValue.Length < 1) id.stringValue = "X";
            id.stringValue = id.stringValue.Substring(0, 1);
            GUILayout.EndHorizontal();

            serializedObject.ApplyModifiedProperties();
        };
    }

    System.Action PositionsField()
    {
        var csz = (ChunkStreamingZone)target;

        var options = new List<GUILayoutOption>();

        options.Add(GUILayout.Height(fieldsHeight));

        var noBoldTitle = new GUIStyle(title);
        noBoldTitle.fontStyle = FontStyle.Normal;

        var icon = Resources.Load<Texture2D>("Editor/Sprites/SPR_D_Flag");

        return delegate {
            GUILayout.BeginHorizontal(options.ToArray());
            GUILayout.Label(new GUIContent(buttonSpace + "Points", icon), noBoldTitle, GUILayout.Height(fieldsHeight), GUILayout.Width(Screen.width * 0.3f));

            if (GUILayout.Button("Add point", style: normalControl)) {
                csz.positions.Add(Vector3.one);
            }

            GUILayout.EndHorizontal();

            serializedObject.ApplyModifiedProperties();
        };
    }
       
    System.Action NeighborsField()
    {
        var csz = (ChunkStreamingZone)target;
        
        var options = new List<GUILayoutOption>();

        options.Add(GUILayout.Height(fieldsHeight));

        var noBoldTitle = new GUIStyle(title);
        noBoldTitle.fontStyle = FontStyle.Normal;

        var icon = Resources.Load<Texture2D>("Editor/Sprites/SPR_D_ChunkNeighborhood");

        return delegate {
            GUILayout.BeginHorizontal(options.ToArray());
            GUILayout.Label(new GUIContent(buttonSpace + "Neighborhood", icon), noBoldTitle, GUILayout.Height(fieldsHeight), GUILayout.Width(Screen.width * 0.3f));

            // Draw property
            GUILayout.BeginVertical();
            var prop = serializedObject.FindProperty("neighborhood");
            prop.Next(true);
            var array = prop.Copy();
            var arrSize = array.arraySize;
            prop.Next(true);
            for (int i = 0; i < arrSize; i++) {
                GUILayout.BeginHorizontal();
                if (!prop.Next(false)) break;

                prop.objectReferenceValue = EditorGUILayout.ObjectField(prop.objectReferenceValue, typeof(ChunkStreamingZone), allowSceneObjects: true);

                // Dupe
                while (prop.objectReferenceValue != null && csz.neighborhood.FindAll(o => o == (ChunkStreamingZone)prop.objectReferenceValue).Count > 1) {
                    csz.neighborhood.Remove((ChunkStreamingZone)prop.objectReferenceValue);
                }

                // Me! Me! Me!
                if (prop.objectReferenceValue == target) prop.objectReferenceValue = null;

                // Remove button
                if (GUILayout.Button("-", GUILayout.Width(20f))) {
                    if (prop.objectReferenceValue == null) array.DeleteArrayElementAtIndex(i);
                    else csz.RemoveNeighbor((ChunkStreamingZone)prop.objectReferenceValue);
                }
                GUILayout.EndHorizontal();
            }
            if (GUILayout.Button("Add", style: normalControl)) {
                array.InsertArrayElementAtIndex(arrSize);
                array.GetArrayElementAtIndex(arrSize).objectReferenceValue = null;
            }
            GUILayout.EndVertical();
                                    
            GUILayout.EndHorizontal();

            // Cleaning of the neighbors
            csz.UpdateNeighborhood();

            serializedObject.ApplyModifiedProperties();
        };
    }

    System.Action ChunkSceneField()
    {
        var csz = (ChunkStreamingZone)target;
        var options = new List<GUILayoutOption>();

        options.Add(GUILayout.Height(fieldsHeight));

        var noBoldTitle = new GUIStyle(title);
        noBoldTitle.fontStyle = FontStyle.Normal;

        var icon = Resources.Load<Texture2D>("Editor/Sprites/SPR_D_ChunkScene");

        return delegate {
            if (!csz.chunk) GUI.color = Color.red;
            GUILayout.BeginHorizontal(options.ToArray());
            GUILayout.Label(new GUIContent(buttonSpace + "Chunk scene", icon), noBoldTitle, GUILayout.Height(fieldsHeight), GUILayout.Width(Screen.width*0.3f));
            serializedObject.FindProperty("chunk").objectReferenceValue = EditorGUILayout.ObjectField(((ChunkStreamingZone)target).chunk, typeof(SceneAsset), allowSceneObjects: true, GUILayout.Height(fieldsHeight));
            serializedObject.ApplyModifiedProperties();
            GUILayout.EndHorizontal();
            GUI.color = Color.white;
        };
    }
}
