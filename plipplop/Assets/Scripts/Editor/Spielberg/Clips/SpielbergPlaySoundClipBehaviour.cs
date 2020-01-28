using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.Playables;

[System.Serializable]
public class SpielbergPlaySoundClipBehaviour : SpielbergClipBehaviour
{
    public string sound;
    [NotKeyable] [Range(0f, 1f)] public float volume = 1f;
    [NotKeyable] public float pitch = 1f;

    public override void ExecuteBehaviour()
    {
        Game.i.cinematics.KinoPlaySound(sound, volume, pitch);
    }

    public override string ToString()
    {
        return "🔊 Play sound\n" + sound;
    }
}
