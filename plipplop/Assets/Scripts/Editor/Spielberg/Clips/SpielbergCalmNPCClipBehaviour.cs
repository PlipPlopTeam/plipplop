using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SpielbergCalmNPCClipBehaviour : SpielbergClipBehaviour
{
    public string npc;

    public override void ExecuteBehaviour()
    {
        Game.i.cinematics.KinoNPCCalm(npc);
    }

    public override string ToString()
    {
        return "AI " + npc + "\nis calmed";
    }
}
