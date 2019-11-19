using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ColliderShape { BOX, SPHERE, CAPSULE }

public class Item : MonoBehaviour, Carryable
{
    [Header("Physics")]
    public ColliderShape shape;
    public BoxCollider bc;
    public SphereCollider sc;
    public CapsuleCollider cc;
    public Rigidbody rb;

    [Header("Item")]
    public GameObject visual;
    [HideInInspector] public bool carried = false;

    public virtual void Start()
    {
        //if(visual == null) Visual(TODO);
    }

    public Transform Self()
    {
        return transform;
    }

    public virtual float Mass()
    {
        if(rb == null) return 0;
        MeshFilter[] meshFilters = gameObject.GetComponentsInChildren<MeshFilter>();
        Vector3 size = Vector3.one;
        foreach(MeshFilter mf in meshFilters)
        {
            if(mf.mesh.bounds.size.magnitude > size.magnitude)
                size = mf.mesh.bounds.size;
        }
        return transform.localScale.magnitude * size.magnitude * rb.mass;
    }

    public virtual void Visual(GameObject go)
    {
        visual = Instantiate(go, transform);

        if(bc != null) Destroy(bc);
        if(sc != null) Destroy(sc);
        if(cc != null) Destroy(cc);

        Mesh m = visual.GetComponentInChildren<MeshFilter>().mesh;
        switch(shape)
        {
            case ColliderShape.BOX:
            bc = gameObject.AddComponent<BoxCollider>();
            bc.size = m.bounds.size;
            break;
            case ColliderShape.SPHERE:
            sc = gameObject.AddComponent<SphereCollider>();
            sc.radius = (m.bounds.size.x + m.bounds.size.y) * 0.5f;
            
            break;
            case ColliderShape.CAPSULE:
            cc = gameObject.AddComponent<CapsuleCollider>();
            cc.height = m.bounds.size.y;
            cc.radius = m.bounds.size.x;
            break;  
        }
    }

    public virtual void Carry()
    {
        carried = true;
        if(sc != null) sc.enabled = false;
        if(bc != null) bc.enabled = false;
        if(rb != null) rb.isKinematic = true;
    }

    public virtual void Drop()
    {
        carried = false;
        if(sc != null) sc.enabled = true;
        if(sc != null) sc.enabled = true;
        if(rb != null) rb.isKinematic = false;
    }
    
    public virtual void Destroy()
    {
        Destroy(gameObject);
    }
}
