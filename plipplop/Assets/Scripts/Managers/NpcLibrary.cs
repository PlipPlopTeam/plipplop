using Behavior;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(menuName = "Behavior/Library")]
public class NpcLibrary : ScriptableObject
{
    [System.Serializable] public class AIStateResource : Library.Resource<AIState> {}

	public List<AIStateResource> states;
    public GameObject NPCSamplePrefab;

	public AIState GetAIStateObject(int id)
	{
		foreach (AIStateResource e in states) if (e.id == id) return e.resource;
		return null;
	}
	public int GetAIStateId(AIState state)
	{
		foreach (AIStateResource e in states) if (e.resource == state) return e.id;
		return -1;
	}
}
