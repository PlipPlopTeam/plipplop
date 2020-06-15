using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SpielbergWaitOnInputClipBehaviour : SpielbergClipBehaviour
{
    public EAction action;

    public override void ExecuteBehaviour()
    {
        Game.i.cinematics.KinoWaitForInput(action);
    }

    public override string ToString()
    {
        return "Wait until "+ action.ToString()+"\nis pressed";
    }
}
