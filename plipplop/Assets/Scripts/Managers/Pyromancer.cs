﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Pyromancer
{
    Dictionary<string, List<VisualEffectController>> effects = new Dictionary<string, List<VisualEffectController>>();
    List<VisualEffectController> allEffects { get { return effects.Values.SelectMany(o => { return o; }).ToList(); } }

    public static void PlayGameEffect(string gfxName, Vector3 position)
    {
        PlayGameEffect(Game.i.library.gfxs[gfxName], position);
    }

    public static void PlayGameEffect(GameFX gfx, Vector3 position)
    {
        foreach (var sound in gfx.sfx) {
            if (sound.spatializedSound) {
                SoundPlayer.PlayAtPosition(sound.name, position, sound.volume, sound.randomPitch);
            }
            else if (sound.randomPitch) {
                SoundPlayer.PlayWithRandomPitch(sound.name, sound.volume);
            }
            else {
                SoundPlayer.Play(sound.name, sound.volume);
            }
        }

        foreach (var vfx in gfx.vfx) {
            PlayVFX(vfx, position);
        }
    }

    public static void PlayGameEffect(GameFX gfx, Transform attach)
    {
        foreach (var sound in gfx.sfx) {
            SoundPlayer.PlaySoundAttached(sound.name, attach, sound.volume, sound.randomPitch);
            if (!sound.spatializedSound) {
                Debug.LogWarning("The sound " + sound.name + " was played ATTACHED even though it is normally NOT spatialized.\nPlease check the calling code.");
            }
        }

        foreach (var vfx in gfx.vfx) {
            PlayVFXAttached(vfx, attach);
        }
    }

    public static void PlayVFX(string vfxName, Vector3 position)
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

    public static void PlayVFXAttached(string vfxName, Transform parent, Vector3 offset=new Vector3())
    {
        var p = Game.i.vfx;
        var effect = p.AddEffect(vfxName);
        effect.Attach(parent);
        effect.transform.localPosition = offset;
    }
}
