using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SpielbergScreenShakeClipBehaviour : SpielbergClipBehaviour
{
    [UnityEngine.Timeline.NotKeyable] [Range(0f, 0.6f)] public float shakeForce = 0.1f;
    [UnityEngine.Timeline.NotKeyable] [Range(0f, 1f)] public float rumbleForce = 0.1f;
    [UnityEngine.Timeline.NotKeyable] public float shakeTime = 1f;

    public override void ExecuteBehaviour()
    {
        Game.i.cinematics.KinoScreenShake(shakeTime, rumbleForce, shakeForce);
    }

    public override string ToString()
    {
        return "📳 Shake screen\nfor {0} seconds".Format(shakeTime);
    }
}
