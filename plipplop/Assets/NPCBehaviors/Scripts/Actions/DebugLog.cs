using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PP;

public class DebugLog : StateActions
{
    public string log = "LOG";
    public override void Execute(StateManager state)
    {
        Debug.Log(Time.time + " : " + log);
    }
}
