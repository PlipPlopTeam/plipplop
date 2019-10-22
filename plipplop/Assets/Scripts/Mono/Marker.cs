using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public abstract class Marker : MonoBehaviour
{
    internal string iconName = "SPR_GIZ_MARKER.tif";

    void OnDrawGizmos()
    {
        Gizmos.color = Color.Lerp(Color.white, new Color(1f, 1f, 1f, 0f), 0.1f);
        Gizmos.DrawIcon(transform.position, GetIconName() ?? iconName, false);
        Gizmos.DrawLine(transform.position - transform.right, transform.position + transform.right);
        Gizmos.DrawLine(transform.position - transform.up, transform.position + transform.up);
        Gizmos.DrawLine(transform.position - transform.forward, transform.position + transform.forward);
        Gizmos.DrawLine(transform.position + transform.forward, transform.position + transform.right);
        Gizmos.DrawLine(transform.position + transform.forward, transform.position - transform.right);

        DrawAdditionalGizmos();
    }

    internal abstract string GetIconName();
    internal abstract void DrawAdditionalGizmos();
}
