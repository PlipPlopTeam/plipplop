using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnMarker : Marker
{
    public Vector3 position { get { return transform.position; } }

    internal override void DrawAdditionalGizmos()
    {
        Gizmos.color = new Color(0.9f, 0.7f, 0.7f, 0.2f);
        Gizmos.DrawCube(transform.position, new Vector3(1f, 2f, 1f));
        Gizmos.DrawWireCube(transform.position, new Vector3(1f, 2f, 1f));
    }

    internal override string GetIconName()
    {
        return "SPR_GIZ_SPAWN.tif";
    }
}
