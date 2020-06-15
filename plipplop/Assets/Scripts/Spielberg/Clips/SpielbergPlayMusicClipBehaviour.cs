using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SpielbergPlayMusicClipBehaviour : SpielbergClipBehaviour
{
    public string musicName;
    [UnityEngine.Timeline.NotKeyable] public float volume = 1f;
    [UnityEngine.Timeline.NotKeyable] public bool shouldFade = true;

    public override void ExecuteBehaviour()
    {
        Game.i.cinematics.KinoPlayMusic(musicName, volume, shouldFade);
    }

    public override string ToString()
    {
        return "🎶 Play music\n" + musicName.Replace("bgm_", "");
    }
}
