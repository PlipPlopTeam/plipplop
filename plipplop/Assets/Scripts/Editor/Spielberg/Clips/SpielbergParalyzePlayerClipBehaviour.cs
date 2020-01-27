using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SpielbergParalyzePlayerClipBehaviour : SpielbergClipBehaviour
{
    public override void ExecuteBehaviour()
    {
        Game.i.cinematics.KinoParalyzePlayer();
    }

    public override string ToString()
    {
        return "Paralyze player";
    }
}
