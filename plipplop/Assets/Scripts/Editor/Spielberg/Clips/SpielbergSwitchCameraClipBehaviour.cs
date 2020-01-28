using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SpielbergSwitchCameraClipBehaviour : SpielbergClipBehaviour
{
    public string cameraName;

    public override void ExecuteBehaviour()
    {
        Game.i.cinematics.KinoSwitchCamera(cameraName);
    }

    public override string ToString()
    {
        return "🎥 Switch to camera\n" + cameraName;
    }
}
