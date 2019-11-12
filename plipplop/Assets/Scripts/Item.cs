using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour, Carryable
{
    public Collider cd;
    public Rigidbody rb;

    public void Carry()
    {
        if(cd != null) cd.enabled = false;
        if(rb != null) rb.isKinematic = true;
    }

    public void Drop()
    {
        if(cd != null) cd.enabled = true;
        if(rb != null) rb.isKinematic = false;
    }
}
