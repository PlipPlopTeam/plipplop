using Behavior;
using Behavior.Editor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AIStateTransitionNode : Node
{
    public List<Condition> conditions = new List<Condition>();
    public bool disable;

    public AIStateTransitionNode()
    {
        SetExitNodesNumber(2);
    }
}
