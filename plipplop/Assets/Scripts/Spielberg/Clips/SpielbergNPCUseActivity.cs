using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SpielbergNPCUseActivity : SpielbergClipBehaviour
{
    public string npc;
    public string act;

    public override void ExecuteBehaviour()
    {
        Game.i.cinematics.KinoNPCUseActivity(npc, act);
    }

    public override string ToString()
    {
        return "AI " + npc + "\nuse " + act;
    }
}
