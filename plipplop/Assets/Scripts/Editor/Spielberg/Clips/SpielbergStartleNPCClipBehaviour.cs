using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SpielbergStartleNPCClipBehaviour : SpielbergClipBehaviour
{
    public string npc;

    public override void ExecuteBehaviour()
    {
        Game.i.cinematics.KinoNPCPanic(npc);
    }

    public override string ToString()
    {
        return "AI " + npc + "\nis startled";
    }
}
