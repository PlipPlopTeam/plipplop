using Behavior;
using Behavior.Editor;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

[System.Serializable]
public class AIStateNode : Node
{
    public AIState currentAIState;

    public SerializedObject serializedAIState;
    public ReorderableList onFixedList;
    public ReorderableList onUpdateList;
    public ReorderableList onEnterList;
    public ReorderableList onExitList;

    public AIStateNode()
    {
        SetExitNodesNumber(1);
    }

    public AIStateTransitionNode AddTransition()
    {
        return graph.AddTransition(this);
    }

    public void RemoveTransition()
    {
        graph.RemoveTransition(this);
    }

    public AIStateTransitionNode GetTransition()
    {
        return graph.GetTransition(this);
    }

    public override void SetGraph(BehaviorGraph graph)
    {
        base.SetGraph(graph);
        if (currentAIState) currentAIState.SetGraph(graph);
    }
}
