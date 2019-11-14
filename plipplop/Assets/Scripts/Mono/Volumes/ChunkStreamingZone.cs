using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ChunkStreamingZone : MonoBehaviour
{

    [Header("Specific options")]
    public List<Vector3> positions;
    public SceneAsset chunk;
    public List<ChunkStreamingZone> neighbors = new List<ChunkStreamingZone>();

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

    bool isSelected = false;

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

    [ExecuteInEditMode]
    public void RemoveAllNeighbors()
    {
        neighbors.Clear();
    }


    [ExecuteInEditMode]
    public void AddNeighbor(ChunkStreamingZone chunkVol)
    {
        chunkVol.neighbors.AddUnique(chunkVol);

        neighbors.AddUnique(chunkVol);
    }

    void OnDrawGizmos()
    {
        GUIStyle labelStyle = GUI.skin.label;
        labelStyle.normal.textColor = Color.Lerp(baseColor, Color.white, 0.2f); 

        Handles.Label(transform.position + positions.ToArray().Mean(), new GUIContent(neighbors.Count.ToString()), labelStyle);
        if(!isSelected) Draw(new Color(baseColor.r, baseColor.g, baseColor.b, baseAlpha), new Color(baseColor.r, baseColor.g, baseColor.b, baseFillAlpha));
        isSelected = false;
    }

    void OnDrawGizmosSelected()
    {
        isSelected = true;
        Draw(new Color(selectedColor.r, selectedColor.g, selectedColor.b, selectedAlpha), new Color(selectedColor.r, selectedColor.g, selectedColor.b, selectedFillAlpha));
        foreach(var neighbor in neighbors) {
            if (neighbor == null) continue;
            neighbor.Draw(new Color(neighborColor.r, neighborColor.g, neighborColor.b, neighborAlpha), new Color(neighborColor.r, neighborColor.g, neighborColor.b, neighborFillAlpha));
        }
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
}
