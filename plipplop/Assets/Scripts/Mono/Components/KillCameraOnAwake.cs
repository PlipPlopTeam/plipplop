using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillCameraOnAwake : KillOnAwake
{
    internal override void Awake()
    {
        componentsToKill = new Component[] { GetComponent<Camera>() };
        base.Awake();
    }
}
