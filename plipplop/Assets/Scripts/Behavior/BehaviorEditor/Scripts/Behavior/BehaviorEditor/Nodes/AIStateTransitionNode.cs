﻿using Behavior;
using Behavior.Editor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AIStateTransitionNode : Node
{
    public List<Condition> conditions = new List<Condition>();
	public Node outputIfTrue;// { get { return graph.GetNodeWithIndex(exitNodes[0]); } }
	public Node outputIfFalse;// { get { return graph.GetNodeWithIndex(exitNodes[1]); } }
    public bool disable;

    public AIStateTransitionNode()
    {
        SetExitNodesNumber(2);
    }
}
