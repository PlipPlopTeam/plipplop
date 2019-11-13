using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ChunkStreamingVolume : Volume
{
    [Header("Specific options")]
    public List<Vector3> positions;
    public SceneAsset chunk;
    public List<ChunkStreamingVolume> neighbors = new List<ChunkStreamingVolume>();

    readonly Color selectedColor = Color.green;
    readonly float selectedAlpha = 0f;
    readonly float selectedFillAlpha = 0.5f;

    readonly Color neighborColor = Color.red;
    readonly float neighborAlpha = 0f;
    readonly float neighborFillAlpha = 0.2f;

    readonly Color baseColor = Color.blue;
    readonly float baseAlpha = 0.5f;
    readonly float baseFillAlpha = 0.4f;

    public override void OnPlayerEnter(Controller player)
    {

    }

    public override void OnPlayerExit(Controller player)
    {

    }

    internal override void OnDrawGizmos()
    {
        Draw(new Color(baseColor.r, baseColor.g, baseColor.b, baseFillAlpha), new Color(baseColor.r, baseColor.g, baseColor.b, baseAlpha));
    }

    void OnDrawGizmosSelected()
    {
        Draw(new Color(selectedColor.r, selectedColor.g, selectedColor.b, selectedFillAlpha), new Color(selectedColor.r, selectedColor.g, selectedColor.b, selectedAlpha));
        foreach(var neighbor in neighbors) {
            if (neighbor == null) continue;
            neighbor.Draw(new Color(neighborColor.r, neighborColor.g, neighborColor.b, neighborFillAlpha), new Color(neighborColor.r, neighborColor.g, neighborColor.b, neighborAlpha));
        }
    }

    public void Draw(Color wireColor, Color fillColor)
    {
        Handles.matrix = transform.localToWorldMatrix;
        Handles.color = wireColor;
        var lines = GetBasePolygon();
        foreach(var line in lines) {
            Handles.DrawLine(line.Item1, line.Item2);
            Handles.DrawLine(line.Item1 + Vector3.up*height, line.Item2 + Vector3.up * height);
            Handles.DrawLine(line.Item1, line.Item1 + Vector3.up * height);
        }
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
}
