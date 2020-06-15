using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SpielbergNPCGoTo : SpielbergClipBehaviour
{
    public string npc;
    public string target;

    public override void ExecuteBehaviour()
    {
        Game.i.cinematics.KinoNPCGoTo(npc, target);
    }

    public override string ToString()
    {
        return "AI " + npc + "\ngo to " + target;
    }
}
