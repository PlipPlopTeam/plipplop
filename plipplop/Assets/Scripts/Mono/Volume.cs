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
    internal Vector3 offset = new Vector3();

    internal virtual void OnDrawGizmos()
    {
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.color = color;
        Gizmos.DrawWireCube(offset, GetSize());
        Gizmos.color = new Color(color.r, color.g, color.b, 0.2f*color.a);
        Gizmos.DrawCube(offset, GetSize());
        Gizmos.color = color;
        Gizmos.matrix = transform.worldToLocalMatrix;
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



    public class Body
    {
        public readonly Rigidbody rigidbody;
        public readonly List<Collider> colliders = new List<Collider>();

        public Body(Rigidbody rigidbody, Collider collider)
        {
            this.rigidbody = rigidbody;
            this.colliders.Add(collider);
        }
    }

    public class ImmergedBodies : List<Body>
    {
        public bool Contains(Rigidbody body)
        {
            return Find(o => o.rigidbody == body) != null;
        }

        public bool Contains(Collider body)
        {
            return Find(o => o.colliders.Contains(body)) != null;
        }

        public void Add(Rigidbody b, Collider c)
        {
            if (Contains(b)) {
                var cols = Find(o => o.rigidbody == b).colliders;
                cols.RemoveAll(o => o == c);
                cols.Add(c);
            }
            else {
                Add(new Body(b, c));
            }
        }

        public bool Remove(Collider c)
        {
            var b = Find(o => o.colliders.Contains(c));
            if (b == null) return true;
            b.colliders.RemoveAll(o => o == c);
            if (b.colliders.Count <= 0f) return true;
            return false;
        }

        public void Remove(Rigidbody r)
        {
            RemoveAll(o => o.rigidbody == r);
        }
    }

}
