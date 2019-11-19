using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillOnAwake : MonoBehaviour
{
    public Component[] componentsToKill;
    internal virtual void Awake()
    {
        foreach(var c in componentsToKill) {
            Destroy(c);
        }
        Destroy(this);
    }
}
