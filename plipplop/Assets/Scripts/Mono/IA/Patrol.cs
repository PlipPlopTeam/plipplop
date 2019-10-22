using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PP;
using UnityEngine.AI;

[CreateAssetMenu(menuName = "Behavior/Action/Patrol")]
public class Patrol : StateActions
{
	public float speed = 5f;
	public float navTreshold = 1f;
	public bool loop = true;
	public Vector3[] path;

	bool destinationReached;
	int point = 0;
	NavMeshAgent agent;

	private void OnEnable()
	{
		point = 0;
		destinationReached = false;
	}

	public override void Execute(StateManager states)
	{
		// RESET
		if (destinationReached)
			destinationReached = false;

		if(agent == null)
		{
			agent = states.transform.gameObject.GetComponentInChildren<NavMeshAgent>();
		}
		else
		{
			if (Vector3.Distance(states.transform.position, agent.destination) <= navTreshold)
			{
				destinationReached = true;
				Go(Next());
			}
		}
	}

	void Go(int index)
	{
		if (path.Length == 0) return;
		agent.SetDestination(path[point]);
		point = index;
	}

	int Next()
	{
		int waypoint = point + 1;
		if (waypoint >= path.Length) waypoint = 0;
		return waypoint;
	}

	bool GoThere(NavMeshAgent agent, Vector3 pos)
	{
		NavMeshPath path = new NavMeshPath();
		agent.CalculatePath(pos, path);
		if (path.status == NavMeshPathStatus.PathPartial
		|| path.status == NavMeshPathStatus.PathInvalid)
		{
			return false;
		}
		else
		{
			agent.SetDestination(pos);
			return true;
		}
	}
}
