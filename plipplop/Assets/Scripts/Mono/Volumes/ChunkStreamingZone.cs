using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChunkStreamingZone : MonoBehaviour
{
    public string identifier;
    public List<Vector3> positions;
    public SceneAsset chunk;    
    public List<ChunkStreamingZone> neighborhood = new List<ChunkStreamingZone>();

    readonly Color selectedColor = Color.green;
    readonly float selectedAlpha = 0.5f;
    readonly float selectedFillAlpha = 0.0f;

    readonly Color neighborColor = Color.yellow;
    readonly float neighborAlpha = 0.5f;
    readonly float neighborFillAlpha = 0.0f;

    readonly Color baseColor = Color.blue;
    readonly float baseAlpha = 0.3f;
    readonly float baseFillAlpha = 0.4f;

    readonly float visualHeight = 50f;

    // Chunk play
    public bool isPlayerInside = false;
    public Scene? scene = null;

    // Wrong references
    bool isDisabled = false;

    // Editor
    bool isSelected = false;

    private void Awake()
    {
        if (!chunk) {
            Debug.LogWarning("Chunk " + identifier + " has NO SCENE ASSET linked to it! Disabling chunk zone.");
            isDisabled = true;
            return;
        }
    }

    private void Start()
    {
        if (!isDisabled) {
            Game.i.chunkLoader.Register(this);
        }
    }

    void LoadChunk()
    {
        SceneManager.LoadSceneAsync(chunk.name, LoadSceneMode.Additive);
    }

    void UnloadChunk()
    {
        
    }


    Tuple<Vector3, Vector3>[] GetBasePolygon()
    {
        var lines = new List<Tuple<Vector3, Vector3>>();

        for (int i = 0; i < positions.Count; i++) {
            Vector3 a, b;
            if (i == 0) {
                a = positions[positions.Count - 1];
            }
            else {
                a = positions[i - 1];
            }
            b = positions[i];

            // Make sure the polygon is a 2D polygon, flattened to ground
            a.y = 0f;
            b.y = 0f;

            lines.Add(new Tuple<Vector3, Vector3>(a, b));
        }

        return lines.ToArray();
    }

    public void RemoveAllNeighbors()
    {
        neighborhood.Clear();
    }

    public void UpdateNeighborhood()
    {
        foreach (var n in neighborhood.ToArray()) {
            if (n == null) continue;
            if (!n.neighborhood.Contains(this)) n.neighborhood.Add(this);
        }
    }

    public void RemoveNeighbor(ChunkStreamingZone csz)
    {
        csz.neighborhood.RemoveAll(o => o == this);
        neighborhood.RemoveAll(o => o == csz);
    }

    void OnDrawGizmos()
    {
        var content = new StringBuilder();
        content.AppendLine(identifier);
        if (isPlayerInside) content.AppendLine("<PLAYER INSIDE>");
        if (chunk!=null && scene != null) content.AppendLine("<LOADED>");

        GUIStyle labelStyle = GUI.skin.label;
        labelStyle.normal.textColor = Color.Lerp(baseColor, Color.white, 0.2f); 

        Handles.Label(transform.TransformPoint(positions.ToArray().Mean()), new GUIContent(content.ToString()), labelStyle);
        if(!isSelected) Draw(new Color(baseColor.r, baseColor.g, baseColor.b, baseAlpha), new Color(baseColor.r, baseColor.g, baseColor.b, baseFillAlpha));
        isSelected = false;
    }

    void OnDrawGizmosSelected()
    {
        isSelected = true;
        Draw(new Color(selectedColor.r, selectedColor.g, selectedColor.b, selectedAlpha), new Color(selectedColor.r, selectedColor.g, selectedColor.b, selectedFillAlpha));
        foreach(var neighbor in neighborhood) {
            if (neighbor == null) continue;
            neighbor.Draw(new Color(neighborColor.r, neighborColor.g, neighborColor.b, neighborAlpha), new Color(neighborColor.r, neighborColor.g, neighborColor.b, neighborFillAlpha));
        }
    }

    public bool IsInsideChunk(Vector3 worldPosition)
    {
        Vector3 position = transform.InverseTransformPoint(worldPosition);
        Vector2 position2d = new Vector2(position.x, position.z);
        var inside = false;
        for (int i = 0; i < positions.Count; i++) {
            var p1 = positions[i];
            var p2 = i < positions.Count - 1 ? positions[i + 1] : positions[0];
            var p3 = i < positions.Count - 2 ? positions[i + 2] : positions[1];
            inside = inside || Geometry.IsPointInTriangle(position2d, new Vector2(p1.x, p1.z), new Vector2(p2.x, p2.z), new Vector2(p3.x, p3.z));
        }
        return inside;
    }

    public void Draw(Color wireColor, Color fillColor)
    {
        Handles.matrix = transform.localToWorldMatrix;
        Handles.color = wireColor;
        var lines = GetBasePolygon();
        foreach(var line in lines) {
            Handles.DrawLine(line.Item1, line.Item2);
            Handles.DrawLine(line.Item1 + Vector3.up* visualHeight, line.Item2 + Vector3.up * visualHeight);
            Handles.DrawLine(line.Item1, line.Item1 + Vector3.up * visualHeight);
        }

        /*
        // Polyfill - not working

        List<Vector3> vertices = new List<Vector3>();

        foreach(var line in lines) {
            vertices.Add(line.Item1);
        }

        for (int i = 0; i < lines.Length; i++) {
            var one = (vertices.FindIndex(o => o == lines[i].Item1));
            var two = (vertices.FindIndex(o => o == lines[i].Item2));
            var next = i == lines.Length - 1 ? lines[0] : lines[i + 1];
            var three = (vertices.FindIndex(o => o == next.Item1));
            Handles.DrawSolidRectangleWithOutline(new Rect(one, two, three), fillColor, wireColor);
        }
        */
    }

    public bool IsLoaded()
    {
        return scene != null;
    }
}
