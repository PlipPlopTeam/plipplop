using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SpielbergWaitOnEndOfDialogueClipBehaviour : SpielbergClipBehaviour
{
    public override void ExecuteBehaviour()
    {
        Game.i.cinematics.KinoWaitEndOfDialogue();
    }

    public override string ToString()
    {
        return "Wait until the end\nof the dialogue";
    }
}
