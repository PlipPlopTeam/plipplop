using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class VisualEffect
{
    public string name;
    public GameObject prefab;
    
    public IVisualEffectController Instantiate()
    {
        var instance = UnityEngine.Object.Instantiate(prefab);
        if (instance.GetComponent<ParticleSystem>())
        {
            return instance.AddComponent<ShurikenEffectController>();

        }
        return instance.AddComponent<DecalEffectController>();

    }
}
