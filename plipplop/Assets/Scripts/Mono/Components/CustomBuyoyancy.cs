using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CustomBuyoyancy : MonoBehaviour
{
    [Range(0f, 50f)] public float buyoyancy = 10f;
    [Range(-2f, 2f)] public float waterline = 0.4f;

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.Lerp(Color.red, Color.yellow, 0.5f);
        Gizmos.DrawWireCube(transform.position - Vector3.up * waterline, 2f * (Vector3.one - Vector3.up));
        Gizmos.color = new Color(Gizmos.color.r, Gizmos.color.g, Gizmos.color.b, 0.2f);
        Gizmos.DrawCube(transform.position - Vector3.up * waterline, 2f * (Vector3.one - Vector3.up));
    }
}
