using Behavior;
using Behavior.Editor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AIStateNode : Node
{
    public AIState state;
    //public SerializedObject serializedAIState;
    public List<AIAction> onFixedList;
    public List<AIAction> onUpdateList;
    public List<AIAction> onEnterList;
    public List<AIAction> onExitList;

    public AIStateNode()
    {
        SetExitNodesNumber(1);
    }

    public AIStateTransitionNode AddTransition()
    {
        return graph.AddTransition(this);
    }
}
