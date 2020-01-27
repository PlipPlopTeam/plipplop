using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SpielbergStopMusicClipBehaviour : SpielbergClipBehaviour
{
    public string musicName;
    public bool shouldFade = true;

    public override void ExecuteBehaviour()
    {
        Game.i.cinematics.KinoStopMusic(musicName, shouldFade);
    }

    public override string ToString()
    {
        return "Fade out "+musicName;
    }
}
