using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

[Serializable]
public class SpielbergClipBehaviour : PlayableBehaviour
{
    public Spielberg.EAction action;
    public string[] args = new string[]{};

    public override void OnBehaviourPlay(Playable playable, FrameData info)
    {
        // Only invoke if time has passed to avoid invoking
        // repeatedly after resume
        if ((info.frameId == 0) || (info.deltaTime > 0)) {
            Game.i.cinematics.Bind(action, args);
        }
    } 
}
