using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportVolume : Volume
{

    [Header("Specific options")]
    public Transform landingPoint;

    public override void OnPlayerEnter(Controller player)
    {
        player.transform.position = landingPoint.position;
        player.transform.forward = landingPoint.forward;
    }

    public override void OnPlayerExit(Controller player)
    {

    }
}
