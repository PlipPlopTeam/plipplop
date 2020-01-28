using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SpielbergPlaySoundClipBehaviour : SpielbergClipBehaviour
{
    public string sound;
    [Range(0f, 1f)] public float volume = 1f;
    public float pitch = 1f;

    public override void ExecuteBehaviour()
    {
        Game.i.cinematics.KinoPlaySound(sound, volume, pitch);
    }

    public override string ToString()
    {
        return "Play sound " + sound;
    }
}
