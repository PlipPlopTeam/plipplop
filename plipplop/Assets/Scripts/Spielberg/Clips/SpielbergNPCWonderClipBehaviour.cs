using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SpielbergNPCWonderClipBehaviour : SpielbergClipBehaviour
{
    public string npcName;

    public override void ExecuteBehaviour()
    {
        Game.i.cinematics.KinoNPCWonder(npcName);
    }

    public override string ToString()
    {
        return "Surprise NPC\n"+ npcName;
    }
}
