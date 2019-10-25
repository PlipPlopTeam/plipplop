using UnityEngine;

public class Valuable : MonoBehaviour
{
    [HideInInspector] public Vector3 origin;
    
    [Header("Settings")]
    public float weight = 1f;
    public float distanceThreshold = 1f;
    public bool hidden = false;

    void Start()
    {
        origin = transform.position;
    }

    public bool HasMoved()
    {
        return Vector3.Distance(origin, transform.position) > distanceThreshold;
    }
}
