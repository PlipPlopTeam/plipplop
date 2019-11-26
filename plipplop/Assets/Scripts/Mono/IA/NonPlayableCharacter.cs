using System.Collections.Generic;
using UnityEngine;
using PP;
using UnityEditor;
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
	[HideInInspector] public Face face;
	
	[Header("Read-Only")]
	public Valuable valuable;
	public Activity activity;
	public Chair chair;
	public Food food;
	public Feeder feeder;
	
	[HideInInspector] public Activity previousActivity;
	[HideInInspector] public ICarryable carried;
	private ICarryable carryableToCollect;
	public Dictionary<Clothes.ESlot, Clothes> clothes = new Dictionary<Clothes.ESlot, Clothes>();

	[Header("Settings")]
	public float strength = 1f;
	float assHeightWhenSitted = 0.51f;

	[Header("Stats")]
	public Dictionary<string, float> stats = new Dictionary<string, float>();

	public ClothesData[] headStuff;
	public ClothesData[] torsoStuff;
	public ClothesData[] legsStuff;

	// Wait timer
	[HideInInspector] public bool hasWaited;
	private float waitTimer;
	private bool endWait;

	public System.Action onWaitEnded;

	public override void Update()
	{
        StartWaiting();
		base.Update();
		if(carried != null && carried.Mass() > strength) StartCarrying(carried);
        StartCollecting();
	}

	private void StartWaiting()
	{
		if(hasWaited) hasWaited = false;
		if(!endWait)
		{
			if(waitTimer > 0) waitTimer -= Time.deltaTime;
			else
			{
				if(onWaitEnded != null)
				{
					onWaitEnded.Invoke();
					onWaitEnded = null;
				}

				endWait = true;
				hasWaited = true;
			}
		}
	}

	public void Wait(float time = 1f, System.Action end = null)
	{
		waitTimer = time;
		endWait = false;
		onWaitEnded = end;
	}

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
		face = GetComponent<Face>();
	}

	public override void Start()
	{
		// Load Character Clothes Slots
		foreach (Clothes.ESlot suit in (Clothes.ESlot[]) Clothes.ESlot.GetValues(typeof(Clothes.ESlot)))
			clothes.Add(suit, null);

		// Load Character Stats
		stats.Add("boredom", 50f);
		stats.Add("tiredness", 50f);
		stats.Add("hunger", 75f);
		
		// Debug Equip Items
		Equip(headStuff[Random.Range(0, headStuff.Length)]);
		Equip(torsoStuff[Random.Range(0, torsoStuff.Length)]);
		Equip(legsStuff[Random.Range(0, legsStuff.Length)]);

		base.Start();
	}

	[ContextMenu("SetHungerTwentyFive")]
	public void SetHungerTwentyFive()
	{
		SetStat("hunger", 25f);
	}
	[ContextMenu("SetHungerSeventyFive")]
	public void SetHungerSeventyFive()
	{
		SetStat("hunger", 75f);
	}
	[ContextMenu("SetHungerHundred")]
	public void SetHungerHundred()
	{
		SetStat("hunger", 100f);
	}

	public void Equip(ClothesData clothData, bool change = true)
	{
		Clothes c = clothes[clothData.slot];
		if(c != null && change) c.Pulverize();

		GameObject clothObject = new GameObject();
		clothObject.name = clothData.name;
		c = clothObject.AddComponent<Clothes>();
		c.Create(clothData, skeleton);
		clothes[clothData.slot] = c;
	}

	public bool IsCarrying(ICarryable carryable)
	{
		return carryable == carried;
	}

	public void StartCarrying(ICarryable carryable)
	{
		carryable.Self().position = (skeleton.GetSocketBySlot(Clothes.ESlot.RIGHT_HAND).GetPosition() + skeleton.GetSocketBySlot(Clothes.ESlot.LEFT_HAND).GetPosition())/2f;
		carryable.Self().forward = transform.forward;
	}

	public void StartCollecting()
	{
		if(carryableToCollect != null)
		{
			if(range.IsInRange(carryableToCollect.Self().gameObject))
			{
				agentMovement.StopChase();
				Carry(carryableToCollect);
				carryableToCollect = null;
			}
		}
	}

	public bool IsCollecting(ICarryable carryable)
	{
		return carryable == carryableToCollect;
	}

	public void Collect(ICarryable carryable)
	{
		carryableToCollect = carryable;
		agentMovement.Chase(carryableToCollect.Self());		
	}

	public void Consume(Food f)
	{
		food = f;
		food.Consume();
		food.onConsumeEnd += () =>
		{
			stats["hunger"] -= food.data.calory;
			Drop();
			food = null;
		};
	}

	public void Carry(ICarryable carryable)
	{
		if(carried != null) Drop();
		carried = carryable;
		carried.Carry();
		if(carried.Mass() <= strength)
		{
			//animator.SetBool("Holding", true);
			skeleton.Attach(carried.Self(), Clothes.ESlot.RIGHT_HAND, true);
		}
		else 
		{
			animator.SetBool("Carrying", true);
		}
	}

	public void Drop()
	{
		if(carried == null) return;
		carried.Drop();

		if(carried.Mass() <= strength)
			skeleton.Drop(Clothes.ESlot.RIGHT_HAND);

		//animator.SetBool("Holding", false);
		animator.SetBool("Carrying", false);

		carried = null;
	}

	public void AddToStat(string name, float amount)
	{
		stats[name] += amount;
		if(stats[name] >= 100f) stats[name] = 100f;
		else if(stats[name] < 0f) stats[name] = 0f;
	}
	public void SetStat(string name, float value)
	{
		stats[name] = value;
		if(stats[name] >= 100f) stats[name] = 100f;
		else if(stats[name] < 0f) stats[name] = 0f;
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
		chair = c;
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

#if UNITY_EDITOR
	void OnDrawGizmosSelected()
    {
        if(EditorApplication.isPlaying)
		{
			float h = 0f;
			Handles.Label(transform.position + Vector3.up * (2f + h), currentState.name);
			h+= 0.1f;
			foreach(KeyValuePair<string, float> entry in stats)
			{
				Handles.Label(transform.position + Vector3.up * (2f + h), entry.Key.ToString() + " = " + entry.Value.ToString());
				h+= 0.1f;
			}
        }
    }
#endif
}