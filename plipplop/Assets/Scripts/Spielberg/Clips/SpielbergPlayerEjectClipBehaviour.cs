using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SpielbergPlayerEjectClipBehaviour : SpielbergClipBehaviour
{
    public override void ExecuteBehaviour()
    {
        if (!Game.i.player.IsPossessingBaseController()) {
            Game.i.player.TeleportBaseControllerAndPossess();
        }
    }

    public override string ToString()
    {
        return "Eject player";
    }
}
