using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SpielbergDisableNPC : SpielbergClipBehaviour
{
    public string npc;

    public override void ExecuteBehaviour()
    {
        Game.i.cinematics.KinoDisableNPC(npc);
    }

    public override string ToString()
    {
        return "Disable AI " + npc;
    }
}
