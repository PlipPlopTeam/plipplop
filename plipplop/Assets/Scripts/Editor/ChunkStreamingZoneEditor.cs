using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ChunkStreamingZone)), CanEditMultipleObjects]
[ExecuteInEditMode]
public class ChunkStreamingZoneEditor : BaseEditor
{
    GUIStyle redStyle;

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

            if (wrongAngle && points[i] != op) {
                var distanceFromAToPoint = Vector3.Distance(previousPoint, points[i]);
                var totalDistanceToPoint = distanceFromAToPoint + Vector3.Distance(nextPoint, points[i]);

                var percentageDistanceFromATP = distanceFromAToPoint / totalDistanceToPoint;

                var distanceFromAToB = Vector3.Distance(previousPoint, nextPoint);
                var newDistance = distanceFromAToB * percentageDistanceFromATP;
                var pointDirection = (nextPoint - previousPoint);

                points[i] = previousPoint + pointDirection.normalized * newDistance;
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
            if (points.Length > 3) {
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
            List<Object> knownNeighbors = new List<Object>();
            List<int> toDestroy = new List<int>();

            for (int i = 0; i < arrSize; i++) {
                GUILayout.BeginHorizontal();
                if (!prop.Next(false)) break;

                prop.objectReferenceValue = EditorGUILayout.ObjectField(prop.objectReferenceValue, typeof(ChunkStreamingZone), allowSceneObjects: true);

                // Dupe
                if (knownNeighbors.Contains(prop.objectReferenceValue)) {
                    toDestroy.Add(i);
                }

                // Me! Me! Me!
                if (prop.objectReferenceValue == target) {
                    toDestroy.Add(i);
                }

                // Remove button
                if (GUILayout.Button("-", GUILayout.Width(20f))) {
                    toDestroy.Add(i);
                }

                // OK, valid neighbor
                if (prop.objectReferenceValue != null) {
                    knownNeighbors.Add(prop.objectReferenceValue);
                }
                GUILayout.EndHorizontal();
            }

            // Cleaning
            array = serializedObject.FindProperty("neighborhood");
            array.Next(true);
            int shift = 0;
            for (int i = 0; i < arrSize; i++) {
                if (toDestroy.Contains(i+shift)) {
                    array.DeleteArrayElementAtIndex(i);
                    if (array.arraySize == arrSize) array.DeleteArrayElementAtIndex(i);
                    i--;
                    arrSize--;
                    shift++; 
                }
            }
            arrSize = knownNeighbors.Count;

            if (GUILayout.Button("Add", style: normalControl)) {
                array.InsertArrayElementAtIndex(arrSize);
                array.GetArrayElementAtIndex(arrSize).objectReferenceValue = null;
            }
            GUILayout.EndVertical();
                                    
            GUILayout.EndHorizontal();

            // Reciprocity check
            array = serializedObject.FindProperty("neighborhood");
            array.Next(true);
            for (int i = 0; i < array.arraySize; i++) {
                var otherChunk = array.GetArrayElementAtIndex(i).objectReferenceValue;
                if (otherChunk == null) {
                    EditorGUILayout.HelpBox("WARNING: Null sector are not allowed in the neighborhood", MessageType.Error, true);
                }
                var otherChunkDeserialized = (ChunkStreamingZone)otherChunk;
                if (!otherChunkDeserialized.neighborhood.Contains((ChunkStreamingZone)target)) {
                    EditorGUILayout.HelpBox("WARNING: Neighbor ["+otherChunkDeserialized.identifier+"] doesn't have chunk ["+csz.identifier+"] registered in its neighborhood", MessageType.Warning, true);
                }
            }



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
            GUILayout.Label(new GUIContent(buttonSpace + "Chunk", icon), noBoldTitle, GUILayout.Height(fieldsHeight), GUILayout.Width(Screen.width*0.3f));
            serializedObject.FindProperty("chunk").objectReferenceValue = EditorGUILayout.ObjectField(((ChunkStreamingZone)target).chunk, typeof(SceneAsset), allowSceneObjects: true, GUILayout.Height(fieldsHeight));
            serializedObject.ApplyModifiedProperties();
            GUILayout.EndHorizontal();
            GUI.color = Color.white;
        };
    }

    internal override void MakeStyles()
    {
        redStyle = GUI.skin.box;
        redStyle.normal.textColor = Color.red;
        base.MakeStyles();
    }
}
