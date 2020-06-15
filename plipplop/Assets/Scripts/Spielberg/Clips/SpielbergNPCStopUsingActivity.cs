using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SpielbergNPCStopUsingActivity : SpielbergClipBehaviour
{
    public string npc;
    public string act;

    public override void ExecuteBehaviour()
    {
        Game.i.cinematics.KinoNPCStopUsingActivity(npc, act);
    }

    public override string ToString()
    {
        return "AI " + npc + "\nstops using " + act;
    }
}
