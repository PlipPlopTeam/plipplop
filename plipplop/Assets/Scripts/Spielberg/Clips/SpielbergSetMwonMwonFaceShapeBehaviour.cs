using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SpielbergSetMwonMwonFaceShapeBehaviour : SpielbergClipBehaviour
{
    public string shapeName;
    [UnityEngine.Timeline.NotKeyable] public float shapeAmount = 0f;

    public override void ExecuteBehaviour()
    {
        Game.i.cinematics.KinoSetBlendShape("SK_MwonMwonExterior", -475048, shapeName, shapeAmount);
    }

    public override string ToString()
    {
        return "Change MM face\n"+shapeName+" to "+shapeAmount;
    }
}
