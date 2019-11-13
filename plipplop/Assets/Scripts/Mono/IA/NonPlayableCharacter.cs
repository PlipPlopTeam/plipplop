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
	private Transform objectToCollect;
	private Chair chair;

	public Skeleton.Socket[] slots;

	[Header("Settings")]
	public float strength = 1f;
	[Range(0f, 100f)] public float boredom = 0f;
	float assHeightWhenSitted = 0.51f;

	void Awake()
	{
		skeleton = GetComponentInChildren<Skeleton>();
		sight = GetComponent<Sight>();
		look = GetComponent<FocusLook>();
		agent = GetComponent<NavMeshAgent>();
		animator = GetComponentInChildren<Animator>();
		range = GetComponent<Range>();

		emo = GetComponent<EmotionRenderer>();
		emo.Load();

		agentMovement = GetComponent<AgentMovement>();
		agentMovement.animator = animator;
	}

	public bool IsCarrying(Transform t)
	{
		return t == inHand;
	}

	public override void Update()
	{
		base.Update();
		if(inHand != null) Carrying(inHand.transform);
		Collecting();
	}

	public void Carrying(Transform t)
	{
		t.position = (skeleton.rightHandBone.position + skeleton.leftHandBone.position)/2f;
		t.forward = transform.forward;
	}

	public void Collecting()
	{
		if(objectToCollect != null)
		{
			if(range.IsInRange(objectToCollect.gameObject))
			{
				agentMovement.StopChase();
				Carry(objectToCollect);
				objectToCollect = null;
			}
		}
	}

	public void Collect(Transform obj)
	{
		objectToCollect = obj;
		agentMovement.Chase(objectToCollect);		
	}

	public void Carry(Transform obj)
	{
		if(inHand != null) Drop();
		inHand = obj;

		Carryable c = inHand.gameObject.GetComponent<Carryable>();
		if(c != null) c.Carry();

		animator.SetBool("Carrying", true);
	}

	public void Drop()
	{
		if(inHand == null) return;

		animator.SetBool("Carrying", false);
		
		Carryable c = inHand.gameObject.GetComponent<Carryable>();
		if(c != null) c.Drop();

		inHand = null;
	}

	public void AddBoredom(float amount)
	{
		boredom += amount;
		if(boredom >= 100f)
		{
			// I'm bored as fuck
			boredom = 100f;
			if(activity != null) activity.Exit(this);
			emo.Show("Bored", 3f);
		}
		else if(boredom < 0f) boredom = 0f;
	}

	public void GoSitThere(Vector3 where)
	{
		agentMovement.GoThere(where);
		agentMovement.onDestinationReached += () => {Sit(where);};
	}
	public void GoSitThere(Chair c, Vector3 offset)
	{

	}
	public void Sit(Vector3 pos)
	{
		agentMovement.Stop();
		agent.enabled = false;

		transform.position = pos;
		animator.SetBool("Sitting", true);
	}
	public void Sit(Chair c, Vector3 offset)
	{
		agentMovement.Stop();
		agent.enabled = false;

		transform.SetParent(c.transform);
		transform.localPosition = new Vector3(offset.x, offset.y - assHeightWhenSitted, offset.z);

		animator.SetBool("Chairing", true);
	}
	public void GetUp()
	{
		if(chair != null)
		{
			transform.SetParent(null);
			chair.Exit(this);
			chair = null;
		}
		agent.enabled = true;
		animator.SetBool("Sitting", false);
		animator.SetBool("Chairing", false);
	}
}