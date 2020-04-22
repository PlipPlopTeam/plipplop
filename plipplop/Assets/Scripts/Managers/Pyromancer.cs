using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Pyromancer
{
    Dictionary<string, List<IVisualEffectController>> effects = new Dictionary<string, List<IVisualEffectController>>();
    List<IVisualEffectController> allEffects { get { return effects.Values.SelectMany(o => { return o; }).ToList(); } }

    public static void PlayGameEffect(string gfxName, Vector3 position)
    {
        PlayGameEffect(Game.i.library.gfxs[gfxName], position);
    }

    public static void PlayGameEffect(GameFX gfx, Vector3 position)
    {
        if(!gfx.randomSound)
        {
            foreach (var sound in gfx.sfx)
            {
                PlayGameEffectSound(sound, position);
            }
        }
        else
        {
            var sound = gfx.sfx.PickRandom();
            PlayGameEffectSound(sound, position);
        }

        if (!gfx.randomEffect)
        {
            foreach (var vfx in gfx.vfx)
            {
                PlayVFX(vfx, position);
            }
        }
        else
        {
            var effect = gfx.vfx.PickRandom();
            PlayVFX(effect, position);
        }
    }

    public static void PlayGameEffectSound(GameFX.SFXParameter sfxParam, Vector3 pos)
    {
        if (sfxParam.spatializedSound)
        {
            SoundPlayer.PlayAtPosition(sfxParam.name, pos, sfxParam.volume, sfxParam.randomPitch);
        }
        else if (sfxParam.randomPitch)
        {
            SoundPlayer.PlayWithRandomPitch(sfxParam.name, sfxParam.volume);
        }
        else
        {
            SoundPlayer.Play(sfxParam.name, sfxParam.volume);
        }
    }

    public static void PlayGameEffect(GameFX gfx, Transform attach, Vector3 offset=new Vector3())
    {
        foreach (var sound in gfx.sfx) {
            SoundPlayer.PlaySoundAttached(sound.name, attach, sound.volume, sound.randomPitch);

            if (!sound.spatializedSound) {
                Debug.LogWarning("The sound " + sound.name + " was played ATTACHED even though it is normally NOT spatialized.\nPlease check the calling code.");
            }
        }

        foreach (var vfx in gfx.vfx) {
            PlayVFXAttached(vfx, attach, offset);
        }
    }

    public static void PlayVFX(string vfxName, Vector3 position)
    {
        var p = Game.i.vfx;
        var effect = p.AddEffect(vfxName);
        effect.SetPosition(position);
    }

    IVisualEffectController AddEffect(string name)
    {
        if (!effects.ContainsKey(name)) {
            effects[name] = new List<IVisualEffectController>();
        }

        var reference = Game.i.library.vfxs.Find(o => o.name == name);
        if (reference == null) {
            throw new Exception(
                "The effect "+name+" DOES NOT EXIST. Check the library."
            );
        }

        var instance = effects[name].Find(o => !o.IsAlive());
        if (instance == null) {
            // Let's create it
            instance = reference.Instantiate();
        }
        else {
            // Let's reuse it
            instance.Activate();
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
        effect.SetLocalPosition(offset);
    }
}
