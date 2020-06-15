using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SpielbergEnableNPC : SpielbergClipBehaviour
{
    public string npc;

    public override void ExecuteBehaviour()
    {
        Game.i.cinematics.KinoEnableNPC(npc);
    }

    public override string ToString()
    {
        return "Enable AI " + npc;
    }
}
