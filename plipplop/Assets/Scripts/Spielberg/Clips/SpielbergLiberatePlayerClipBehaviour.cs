using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SpielbergLiberatePlayerBehaviour : SpielbergClipBehaviour
{

    public override void ExecuteBehaviour()
    {
        Game.i.cinematics.KinoLiberatePlayer();
    }

    public override string ToString()
    {
        return "🕊 Liberate player";
    }
}
