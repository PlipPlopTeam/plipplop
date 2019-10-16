using UnityEngine;

public class Valuable : MonoBehaviour
{
    [HideInInspector]
    public Vector3 origin;
    public float distanceThreshold = 1f;

    void Start()
    {
        origin = transform.position;
    }

    public bool HasMoved()
    {
        return Vector3.Distance(origin, transform.position) > distanceThreshold;
    }
}
