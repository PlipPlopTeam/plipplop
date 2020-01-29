using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SpielbergPlayGameEffectClipBehaviour : SpielbergClipBehaviour
{
    public string gfx;
    [UnityEngine.Timeline.NotKeyable] public Vector3 position;

    public override void ExecuteBehaviour()
    {
        Game.i.cinematics.KinoPlayGameEffect(gfx, position);
    }

    public override string ToString()
    {
        return "💥 Play GameFX\n" + gfx;
    }
}
