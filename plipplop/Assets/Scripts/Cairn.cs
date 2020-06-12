using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cairn : MonoBehaviour
{
    public List<Rigidbody> cailloux;
    
    private void OnTriggerEnter(Collider other)
    {
        if (!other.GetComponent<Controller>())
        {
            return;
        }
        if (cailloux.Count >= 0)
        {
            foreach (var _r in cailloux)
            {
                _r.isKinematic = false;
            }

            cailloux.Clear();
        }
        gameObject.SetActive(false);
    }
}
