using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SpielbergTeleportPlayerBehaviour : SpielbergClipBehaviour
{

    public Vector3 teleportPosition;
    public Quaternion teleportRotation;

    public override void ExecuteBehaviour()
    {
        var controller = Game.i.player.GetCurrentController();
        controller.transform.position = teleportPosition;
        controller.transform.rotation = teleportRotation;
    }

    public override string ToString()
    {
        return "Teleport player\nto "+teleportPosition+"";
    }
}
