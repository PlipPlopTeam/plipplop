using UnityEngine;
public class Item : MonoBehaviour, ICarryable
{
	public enum EColliderShape { BOX, SPHERE, CAPSULE }
	public enum EType { TRASH, FOOD, ABSTRACT, OTHER }

    [Header("Item")]
    public bool physiqued = true;
    public EColliderShape shape;
    public Rigidbody rb;
	public new Collider collider;
    [Space(5)]
	public EType type = EType.OTHER;
    public float weight = 1f;
    public GameObject visuals;

    private bool carried = false;
	public bool IsCarried() { return carried; }

	public virtual void Awake()
    {
        if(physiqued)
        {
            if (rb == null) rb = GetComponent<Rigidbody>();
            if (rb == null) rb = gameObject.AddComponent<Rigidbody>();
        }
        if(carried) Carry();
        else Drop();
    }

    public Transform Self()
    {
        return transform;
    }

    public string Name()
    {
        return GetType().ToString().ToLower();
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
        return transform.localScale.magnitude * size.magnitude * rb.mass * weight;
    }

    public virtual void Visual(GameObject go)
    {
        if (visuals != null) Destroy(visuals);

		visuals = Instantiate(go, transform);
        Mesh m = visuals.GetComponentInChildren<MeshFilter>().mesh;
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

    public virtual void Constraint()
    {
        if (rb != null)
        {
            rb.constraints = RigidbodyConstraints.FreezeAll;
        }
    }

    public virtual void UnConstraint()
    {
        if (rb != null)
        {
            rb.constraints = RigidbodyConstraints.None;
        }
    }

    public virtual void Ghost()
    {
        if (collider != null) collider.enabled = false;
        if (rb != null) rb.isKinematic = true;
        if (rb != null) rb.useGravity = false;
    }

    public virtual void UnGhost()
    {
        if (collider != null) collider.enabled = true;
        if (rb != null) rb.isKinematic = false;
        if (rb != null) rb.useGravity = true;
    }


    public virtual void Carry()
    {
		carried = true;
        Ghost();
    }

    public virtual void Drop()
    {
        carried = false;
        UnGhost();
    }
    
    public virtual void Destroy()
    {
        Destroy(gameObject);
    }
}
