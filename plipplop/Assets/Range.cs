using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Range : MonoBehaviour
{
    public Vector3 offset;
    public float radius;

    public bool IsInRange(GameObject gameObject)
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position + offset, radius);
        foreach(Collider c in colliders)
        {
            if(c.gameObject == gameObject) return true;
        }
        return false;
    }

    internal void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position + offset, radius);
    }
}
