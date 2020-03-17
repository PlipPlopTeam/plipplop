using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticCameraVolume : Volume { 

    [Header("Specific options")]
    public Transform aim;
    public float FOV = 60f;
    public bool isImmediate = false;
    public bool lookAtTarget = false;

    public List<GameObject> objectsToHide = new List<GameObject>();

    int lookAtIndex = 0;
    Geometry.PositionAndRotation objective;

    public override void OnPlayerEnter(Controller player)
    {
        if (isImmediate) {
            //Game.i.aperture.Freeze();
            Game.i.aperture.AddStaticPosition(aim.position, aim.rotation);
            Game.i.aperture.FixedUpdate();
            Game.i.aperture.fieldOfView.destination = FOV;
            Game.i.aperture.Teleport();
        }
        else {
            objective = Game.i.aperture.AddStaticPosition(aim);
        }

        if (!lookAtTarget) 
        {
            lookAtIndex = Game.i.aperture.DisableLookAt();
        }
        else 
        {
            Game.i.aperture.EnableLookAt();
        }

        // TODO : Replace by a fadeout
        foreach (GameObject o in objectsToHide) o.SetActive(false);
    }

    public override void OnPlayerExit(Controller player)
    {
        if (isImmediate) {
            Game.i.aperture.Unfreeze();
        }
        else {
            Game.i.aperture.RemoveStaticPosition(objective);
        }

        if (!lookAtTarget)
        {
            Game.i.aperture.RestoreLookAt(lookAtIndex);
        }

        foreach (GameObject o in objectsToHide) o.SetActive(true);
    }
}
