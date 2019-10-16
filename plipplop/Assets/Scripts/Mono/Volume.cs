using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CollisionEventTransmitter))]
public class Volume : MonoBehaviour
{
    public float height;
    public float width;
    public float length;

    public Vector3 size { get { return GetSize(); } }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0f, 1f, 1f, 0.1f);
        Gizmos.DrawCube(transform.position, GetSize());
        Gizmos.color = new Color(0f, 1f, 1f, 0.5f);
        Gizmos.DrawWireCube(transform.position, GetSize());
    }

    Vector3 GetSize()
    {
        return new Vector3(width, height, length);
    }

    private void Awake()
    {
        transform.localScale = Vector3.one;
        var col = gameObject.AddComponent<BoxCollider>();
        col.size = GetSize();
        col.isTrigger = true;
    }
}
