using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SpielbergPlayDialogueClipBehaviour : SpielbergClipBehaviour
{
    public string dialogue;

    public override void ExecuteBehaviour()
    {
        Game.i.cinematics.KinoStartDialogue(dialogue);
    }

    public override string ToString()
    {
        return "Mwon Mwon plays\n"+dialogue;
    }
}
