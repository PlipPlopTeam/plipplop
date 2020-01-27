using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Pyromancer
{
    Dictionary<string, List<VisualEffectController>> effects = new Dictionary<string, List<VisualEffectController>>();
    List<VisualEffectController> allEffects { get { return effects.Values.SelectMany(o => { return o; }).ToList(); } }

    public static void Play(string vfxName, Vector3 position)
    {
        var p = Game.i.vfx;
        var effect = p.AddEffect(vfxName);
        effect.SetPosition(position);
    }

    VisualEffectController AddEffect(string name)
    {
        if (!effects.ContainsKey(name)) {
            effects[name] = new List<VisualEffectController>();
        }

        var reference = Game.i.library.vfxs.Find(o => o.name == name);
        if (reference == null) {
            throw new Exception(
                "The effect "+name+" DOES NOT EXIST. Check the library."
            );
        }

        var instance = effects[name].Find(o => !o.IsAlive() || !o.gameObject.activeSelf);
        if (instance == null) {
            // Let's create it
            instance = reference.Instantiate();
        }
        else {
            // Let's reuse it
            instance.gameObject.SetActive(true);
            instance.Reset();
        }

        effects[name].Add(instance);

        return instance;
    }

    public static void PlayAttached(string vfxName, Transform parent, Vector3 offset=new Vector3())
    {
        var p = Game.i.vfx;
        var effect = p.AddEffect(vfxName);
        effect.Attach(parent);
        effect.transform.localPosition = offset;
    }
}
