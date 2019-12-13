using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.HDPipeline;

public class KillCameraOnAwake : KillOnAwake
{
    internal override void Awake()
    {
        componentsToKill = new Component[] { GetComponent<HDAdditionalCameraData>() , GetComponent<Camera>()};
        base.Awake();
    }
}
