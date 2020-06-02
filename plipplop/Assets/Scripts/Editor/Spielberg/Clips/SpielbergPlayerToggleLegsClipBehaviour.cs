using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SpielbergPlayerToggleLegsClipBehaviour : SpielbergClipBehaviour
{
    public override void ExecuteBehaviour()
    {
        Game.i.player.GetCurrentController().ToggleLegs();
    }

    public override string ToString()
    {
        return "Toggle player legs";
    }
}
