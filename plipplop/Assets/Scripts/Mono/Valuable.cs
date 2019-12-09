using UnityEngine;

public class Valuable : MonoBehaviour, INoticeable, ICarryable
{
    [HideInInspector] public Vector3 origin;
    
    [Header("Settings")]
    public float weight = 1f;
    public float distanceThreshold = 2f;
    public bool hidden = false;
	private Rigidbody rb;
	private Collider col;

	void Awake()
	{
		rb = GetComponent<Rigidbody>();
		col = GetComponent<Collider>();
	}

	public virtual void Carry()
	{
		if (col != null) col.enabled = false;
		if (rb != null) rb.isKinematic = true;
	}

	public virtual void Drop()
	{
		if (col != null) col.enabled = true;
		if (rb != null) rb.isKinematic = false;
	}

	void Start()
    {
        origin = transform.position;
    }

    public void Notice()
    {
        // Does things..
    }
    public bool IsVisible()
    {
        return !hidden;
    }
    public void SetVisible(bool value)
    {
        hidden = value;
    }

    public bool HasMoved()
    {
        return Vector3.Distance(origin, transform.position) > distanceThreshold;
    }

	public float Mass()
	{
		return weight;
	}

	public Transform Self()
	{
		return transform;
	}
}
