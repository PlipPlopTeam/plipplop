using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PP;
using UnityEngine.AI;

public class Guard : StateManager
{
	Sight sight;
	FocusLook look;
	NavMeshAgent agent;
	Animator animator;
	Valuable thing;
	Skeleton skeleton;
	EmotionRenderer emo;
	CollisionEventTransmitter range;
	List<GameObject> inRange = new List<GameObject>();

	public bool found;

	void Awake()
	{
		skeleton = GetComponentInChildren<Skeleton>();
		sight = GetComponent<Sight>();
		look = GetComponent<FocusLook>();
		agent = GetComponent<NavMeshAgent>();
		animator = GetComponent<Animator>();
		emo = GetComponent<EmotionRenderer>();
		range = GetComponentInChildren<CollisionEventTransmitter>();
		//range.onTriggerEnter += (Collider other) => { inRange.Add(other.transform.gameObject); };
		//range.onTriggerExit += (Collider other) => { inRange.Remove(other.transform.gameObject); };
	}

	public float DistanceToDestination()
	{
		return Vector3.Distance(transform.position, agent.destination);
	}
}
