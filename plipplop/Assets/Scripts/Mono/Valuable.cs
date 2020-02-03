using UnityEngine;

public class Valuable : Item, INoticeable, ICarryable
{
    [HideInInspector] public Vector3 origin;
    
    [Header("Settings")]
    public float weight = 1f;
    public float distanceThreshold = 2f;
    public bool hidden = false;
	private Rigidbody rb;
	private Collider col;

	private bool carried;
	public bool IsCarried() { return carried; }

	void Awake()
	{
		rb = GetComponent<Rigidbody>();
		col = GetComponent<Collider>();
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
}
