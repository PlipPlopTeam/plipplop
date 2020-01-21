using UnityEngine;

public enum EColliderShape { BOX, SPHERE, CAPSULE }

public class Item : MonoBehaviour, ICarryable
{
    [Header("Physics")]
    public EColliderShape shape;
    public Rigidbody rb;
	public Collider collider;

    [Header("Item")]
    public GameObject visual;
    [HideInInspector] public bool carried = false;

    public virtual void Awake()
    {

        if(rb == null) rb = GetComponent<Rigidbody>();
        if(rb == null) rb = gameObject.AddComponent<Rigidbody>();
        if(carried) Carry();
        else Drop();
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
        Mesh m = visual.GetComponentInChildren<MeshFilter>().mesh;
		collider = gameObject.GetComponent<Collider>();
		if (collider == null)
		{
			switch (shape)
			{
				case EColliderShape.BOX:
					BoxCollider c = gameObject.AddComponent<BoxCollider>();
					c.size = m.bounds.size;
					collider = c;
					break;
				case EColliderShape.SPHERE:
					SphereCollider sc = gameObject.AddComponent<SphereCollider>();
					sc.radius = (m.bounds.size.x + m.bounds.size.y) * 0.5f;
					collider = sc;
					break;
				case EColliderShape.CAPSULE:
					CapsuleCollider cc = gameObject.AddComponent<CapsuleCollider>();
					cc.height = m.bounds.size.y;
					cc.radius = m.bounds.size.x;
					collider = cc;
					break;
			}
		}

    }

    public virtual void Carry()
    {
		carried = true;
		if (collider != null) collider.enabled = false;
        if(rb != null) rb.isKinematic = true;
        if(rb != null) rb.useGravity = false;
    }

    public virtual void Drop()
    {
        carried = false;
		if (collider != null) collider.enabled = true;
        if(rb != null) rb.isKinematic = false;
        if(rb != null) rb.useGravity = true;
    }
    
    public virtual void Destroy()
    {
        Destroy(gameObject);
    }
}
