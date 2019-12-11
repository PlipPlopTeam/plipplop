using Behavior;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NpcLibrary
{
    [System.Serializable] public class AIStateResource : Library.Resource<AIState> { }

    public List<AIStateResource> aiStates;
    public GameObject NPCSamplePrefab;

}
