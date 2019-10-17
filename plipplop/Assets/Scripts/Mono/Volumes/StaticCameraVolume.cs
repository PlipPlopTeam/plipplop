using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticCameraVolume : Volume { 

    [Header("Specific options")]
    public Transform aim;

    public override void OnPlayerEnter(Controller player)
    {
        Game.i.aperture.Freeze();
        Game.i.aperture.cam.transform.position = aim.position;
        Game.i.aperture.cam.transform.forward = aim.forward;
    }

    public override void OnPlayerExit(Controller player)
    {
        Game.i.aperture.Unfreeze();
    }
}
