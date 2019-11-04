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
	[HideInInspector] public Skeleton skeleton;
	[HideInInspector] public EmotionRenderer emo;
	[HideInInspector] public Range range;

	[HideInInspector] public Valuable valuable;
	[HideInInspector] public Activity activity;

	public float strength = 1f;

	void Awake()
	{
		skeleton = GetComponentInChildren<Skeleton>();
		sight = GetComponent<Sight>();
		look = GetComponent<FocusLook>();
		agent = GetComponent<NavMeshAgent>();
		animator = GetComponent<Animator>();
		range = GetComponent<Range>();

		emo = GetComponent<EmotionRenderer>();
		emo.Load();

		agentMovement = GetComponent<AgentMovement>();
		agentMovement.animator = animator;
	}
}