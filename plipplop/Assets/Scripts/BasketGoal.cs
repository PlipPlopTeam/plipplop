using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasketGoal : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Ball _b = other.GetComponentInChildren<Ball>();
        
        if (_b!=null)
        {
            Pyromancer.PlayGameEffect("gfx_bottle_flip_success", transform.position);
        }
    }
}
