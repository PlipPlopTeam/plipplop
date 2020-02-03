using UnityEngine;

public class Valuable : Item, INoticeable, ICarryable
{
    [HideInInspector] public Vector3 origin;
    
    [Header("Settings")]
    public float weight = 1f;
    public float distanceThreshold = 2f;
    public bool hidden = false;

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
