using System.Collections.Generic;
using UnityEngine;
using PP;
using UnityEngine.AI;

public class NPC : StateManager
{
	[HideInInspector] public Sight sight;
	[HideInInspector] public FocusLook look;
	[HideInInspector] public NavMeshAgent agent;
	[HideInInspector] public AgentMovement agentMovement;
	[HideInInspector] public Animator animator;
	[HideInInspector] public Valuable thing;
	[HideInInspector] public Skeleton skeleton;
	[HideInInspector] public EmotionRenderer emo;
	[HideInInspector] public CollisionEventTransmitter range;
	[HideInInspector] public List<GameObject> inRange = new List<GameObject>();

	void Awake()
	{
		skeleton = GetComponentInChildren<Skeleton>();
		sight = GetComponent<Sight>();
		look = GetComponent<FocusLook>();
		agent = GetComponent<NavMeshAgent>();
		agentMovement = GetComponent<AgentMovement>();
		animator = GetComponent<Animator>();
		emo = GetComponent<EmotionRenderer>();
		
        range = GetComponentInChildren<CollisionEventTransmitter>();
        range.onTriggerEnter += (Collider other) => { inRange.Add(other.transform.gameObject); };
        range.onTriggerExit += (Collider other) => { inRange.Remove(other.transform.gameObject); };
	}
}

#region Actions
[CreateAssetMenu(menuName = "Behavior/Action/NPC/GoToNextPathPoint")]
public class GoToNextPathPoint : StateActions
{
	public override void Execute(StateManager state)
	{
		NPC npc = (NPC)state;
		if(npc != null) npc.agentMovement.GoToNextPoint();
	}
}

[CreateAssetMenu(menuName = "Behavior/Action/NPC/GoThere")]
public class GoThere : StateActions
{
    public Vector3 position;

	public override void Execute(StateManager state)
	{
		NPC npc = (NPC)state;
		if(npc != null) npc.agentMovement.GoThere(position);
	}
}
[CreateAssetMenu(menuName = "Behavior/Action/NPC/SearchValuable")]
public class SearchValuable : StateActions
{
	public override void Execute(StateManager state)
	{
		NPC npc = (NPC)state;
		if(npc != null)
		{
			Valuable[] items = npc.sight.Scan<Valuable>();
			if(items.Length > 0) npc.thing = items[0];
		}
	}
}
[CreateAssetMenu(menuName = "Behavior/Action/NPC/StopMovement")]
public class StopMovement : StateActions
{
	public override void Execute(StateManager state)
	{
		NPC npc = (NPC)state;
		if(npc != null)
		{
			npc.agentMovement.Stop();
		}
	}
}
[CreateAssetMenu(menuName = "Behavior/Action/NPC/ForgetThing")]
public class ForgetThing : StateActions
{
	public override void Execute(StateManager state)
	{
		NPC npc = (NPC)state;
		if(npc != null)
		{
			npc.thing = null;
		}
	}
}
#endregion

#region Conditions
[CreateAssetMenu(menuName = "Behavior/Condition/NPC/OnDestinationReached")]
public class OnDestinationReached : Condition
{
	public override bool CheckCondition(StateManager state)
	{
		NPC npc = (NPC)state;
		if(npc != null)
		{
			if(npc.agentMovement.reached) return true;
		}
		
        return false;
	}
}

[CreateAssetMenu(menuName = "Behavior/Condition/NPC/HasSeenValuable")]
public class HasSeenValuable : Condition
{
	public override bool CheckCondition(StateManager state)
	{
		NPC npc = (NPC)state;
		if(npc != null)
		{
			return npc.thing == true;
		}
        return false;
	}
}
#endregion