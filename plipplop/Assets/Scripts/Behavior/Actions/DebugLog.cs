﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PP;

[CreateAssetMenu(menuName = "Behavior/Action/DebugLogString")]
public class DebugLogString : StateActions
{
    public string logString = "LOG";
    public override void Execute(StateManager state)
    {
        Debug.Log(Time.time + " : " + logString);
    }
}