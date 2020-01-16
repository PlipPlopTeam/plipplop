using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class VisualEffect
{
    public string name;
    public GameObject prefab;
    
    public VisualEffectController Instantiate()
    {
        var instance = UnityEngine.Object.Instantiate(prefab);
        return instance.AddComponent<VisualEffectController>();
    }
}
