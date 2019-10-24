using System.Collections.Generic;
using UnityEngine;
using PP;
using UnityEngine.AI;

public class NonPlayableCharacter : StateManager
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