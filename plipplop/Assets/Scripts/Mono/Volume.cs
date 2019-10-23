using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CollisionEventTransmitter))]
public abstract class Volume : MonoBehaviour
{
    [Header("Volume options")]
    public Color color = new Color(0f, 1f, 1f);

    public float height=20f;
    public float width=20f;
    public float length=20f;

    public Vector3 size { get { return GetSize(); } }

    bool isInside = false;

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(color.r, color.g, color.b, 0.1f);
        Gizmos.DrawCube(transform.position, GetSize());
        Gizmos.color = new Color(color.r, color.g, color.b, 0.5f);
        Gizmos.DrawWireCube(transform.position, GetSize());
    }

    internal Vector3 GetSize()
    {
        return new Vector3(width, height, length);
    }

    private void Awake()
    {
        transform.localScale = Vector3.one;
        var box = gameObject.AddComponent<BoxCollider>();
        box.isTrigger = true;
        UpdateSize();

        var col = GetComponent<CollisionEventTransmitter>();
        col.onTriggerEnter += Col_onTriggerEnter;
        col.onTriggerExit += Col_onTriggerExit;
    }

    internal void UpdateSize()
    {
        GetComponent<BoxCollider>().size = GetSize();
    }

    private void Col_onTriggerExit(Collider obj)
    {
        OnObjectExit(obj);
        var c = obj.gameObject.GetComponent<Controller>();
        if (isInside && c && Game.i.player.IsPossessing(c)) {
            isInside = false;
            OnPlayerExit(c);
        }
    }

    private void Col_onTriggerEnter(Collider obj)
    {
        OnObjectEnter(obj);
        var c = obj.gameObject.GetComponent<Controller>();
        if (!isInside && c && Game.i.player.IsPossessing(c)) {

            isInside = true;
            OnPlayerEnter(c);
        }
    }

    public virtual void OnObjectEnter(Collider g) {}

    public virtual void OnObjectExit(Collider g) {}

    public abstract void OnPlayerEnter(Controller player);

    public abstract void OnPlayerExit(Controller player);

}
