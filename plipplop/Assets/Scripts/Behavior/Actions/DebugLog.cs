﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Behavior;

[CreateAssetMenu(menuName = "Behavior/Action/DebugLogString")]
public class DebugLogString : AIAction
{
    public string logString = "LOG";
    public override void Execute(NonPlayableCharacter target)
    {
        Debug.Log(Time.time + " : " + logString);
    }
}
