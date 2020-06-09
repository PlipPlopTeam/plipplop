using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SpielbergTeleportPlayerBehaviour : SpielbergClipBehaviour
{

    public Transform teleportationTarget;

    public override void ExecuteBehaviour()
    {
        var controller = Game.i.player.GetCurrentController();
        controller.transform.position = teleportationTarget.position;
        controller.transform.rotation = teleportationTarget.rotation;
    }

    public override string ToString()
    {
        return "Teleport player\nto "+teleportationTarget+"";
    }
}
