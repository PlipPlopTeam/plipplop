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
	[HideInInspector] public Activity previousActivity;
	[HideInInspector] public Transform inHand;

	[Header("Settings")]
	public float strength = 1f;
	[Range(0f, 100f)] public float boredom = 0f;

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

	public override void Update()
	{
		base.Update();

		if(inHand != null)
		{
			inHand.transform.position = (skeleton.rightHandBone.position + skeleton.leftHandBone.position)/2f;
        	inHand.transform.forward = transform.forward;
		}
	}

	public void Carry(Transform obj)
	{
		if(inHand != null) Drop();
		inHand = obj;
		animator.SetBool("Carrying", true);
	}

	public void Drop()
	{
		animator.SetBool("Carrying", false);
		inHand = null;
	}

	public void AddBoredom(float amount)
	{
		boredom += amount;
		if(boredom >= 100f)
		{
			// I'm bored as fuck
			boredom = 100f;
			if(activity != null) activity.Kick(this);
			emo.Show("Bored", 3f);
		}
		else if(boredom < 0f) boredom = 0f;
	}
}