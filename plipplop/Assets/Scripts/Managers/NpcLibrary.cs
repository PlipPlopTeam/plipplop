using Behavior;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(menuName = "Behavior/Library")]
public class NpcLibrary : ScriptableObject
{
	[System.Serializable] public class AIStateResource : Library.Resource<AIState> { }
	[System.Serializable] public class AIConditionResource : Library.Resource<Condition> { }
	[System.Serializable] public class AIActionResource : Library.Resource<AIAction> { }

	public List<AIStateResource> states;
	public List<AIConditionResource> conditions;
	public List<AIActionResource> actions;
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
	public Condition GetConditionObject(int id)
	{
		foreach (AIConditionResource e in conditions) if (e.id == id) return e.resource;
		return null;
	}
	public int GetConditionId(Condition condition)
	{
		foreach (AIConditionResource e in conditions) if (e.resource == condition) return e.id;
		return -1;
	}
}
