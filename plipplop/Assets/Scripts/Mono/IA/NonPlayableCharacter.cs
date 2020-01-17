using System.Collections.Generic;
using UnityEngine;
using Behavior.Editor;
using UnityEngine.AI;

#if UNITY_EDITOR
	using UnityEditor;
#endif

public class NonPlayableCharacter : MonoBehaviour
{
	public enum EStat { BOREDOM, TIREDNESS, HUNGER };
	public enum ESubject { PLAYER, VALUABLE, ACTIVITY, CHAIR, FOOD, FEEDER };

	[HideInInspector] public Sight sight;
	[HideInInspector] public FocusLook look;
	[HideInInspector] public NavMeshAgent agent;
	[HideInInspector] public AgentMovement agentMovement;
	[HideInInspector] public Animator animator;
	[HideInInspector] public Skeleton skeleton;
	[HideInInspector] public EmotionRenderer emo;
	[HideInInspector] public Range range;
	[HideInInspector] public Face face;
	[HideInInspector] public Controller player;
	[HideInInspector] public Valuable valuable;
	[HideInInspector] public Activity activity;
	[HideInInspector] public Activity previousActivity;
	[HideInInspector] public Chair chair;
	[HideInInspector] public Food food;
    [HideInInspector] public Feeder feeder;
	[HideInInspector] public Collider collider;
	[HideInInspector] public ICarryable carried;

	public Dictionary<Clothes.ESlot, Clothes> clothes = new Dictionary<Clothes.ESlot, Clothes>();
	public Dictionary<EStat, float> stats = new Dictionary<EStat, float>();

	[Header("Settings")]
	public NonPlayableCharacterSettings settings;
	public BehaviorGraphData graph;

	float assHeightWhenSitted = 0.51f;
    ICarryable carryableToCollect;
	private float waitTimer;
	private bool endWait;
	[HideInInspector] public bool hasWaited;
	public System.Action onWaitEnded;

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
		collider = GetComponent<Collider>();

		// Load Character Clothes Slots
		foreach (Clothes.ESlot suit in (Clothes.ESlot[])Clothes.ESlot.GetValues(typeof(Clothes.ESlot)))
			clothes.Add(suit, null);
	}

	public void Start()
	{
		// Loading Settings
		if (settings == null) settings = Game.i.library.npcLibrary.defaultSettings;
		skeleton.gameObject.transform.localScale = Vector3.one * settings.height/2;
		stats.Add(EStat.BOREDOM, settings.initialBoredom);
		stats.Add(EStat.TIREDNESS, settings.initialTiredness);
		stats.Add(EStat.HUNGER, settings.initialTiredness);
		foreach (ClothesData c in settings.clothes) Equip(c);
		/*
		foreach (KeyValuePair<Clothes.ESlot, Clothes> c in clothes)
		{
			if (c.Value != null) c.Value.Scale(settings.height / 2);
		}
		*/
		// Loading Graph
		if (graph == null)
		{
			Destroy(gameObject);
			return;
		}
		graph = Instantiate(graph);
		graph.Load(this);
	}

	public virtual void Update()
	{
		UpdateCollecting();
		UpdateWaiting();
		agentMovement.Tick();
		if (carried != null && carried.Mass() > settings.strength) StartCarrying(carried);
		graph.Update();
	}

	public void FixedUpdate()
	{
		graph.FixedUpdate();
	}

	private void UpdateWaiting()
	{
		if (hasWaited) hasWaited = false;
		if (!endWait)
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

	public void Equip(ClothesData clothData, bool change = true)
	{
		Clothes c = clothes[clothData.slot];
		if(c != null && change) c.Destroy();

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
	public bool IsCarrying()
	{
		return carried != null;
	}

	public void StartCarrying(ICarryable carryable)
	{
		carryable.Self().position = (skeleton.GetSocketBySlot(Clothes.ESlot.RIGHT_HAND).GetPosition() + skeleton.GetSocketBySlot(Clothes.ESlot.LEFT_HAND).GetPosition())/2f;
		carryable.Self().forward = transform.forward;
	}

	public void UpdateCollecting()
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
		food.Consume(delegate{
			if(this.food != null)
			{
				this.AddToStat(NonPlayableCharacter.EStat.HUNGER, -this.food.data.calory);
				this.Drop();
				this.food = null;
			}
		});
	}

	public virtual void Carry(ICarryable carryable)
	{
		if (carried != null) Drop();
		carried = carryable;
		carried.Carry();
		if (carried.Mass() <= settings.strength)
		{
			animator.SetBool("Holding", true);
			skeleton.Attach(carried.Self(), Clothes.ESlot.RIGHT_HAND, true);
		}
		else
		{
			animator.SetBool("Carrying", true);
		}
			
	}

	public virtual void Kick(Controller controller)
	{
		if (controller.IsPossessed()) Game.i.player.TeleportBaseControllerAndPossess();
	}

	public void Drop()
	{
		if(carried == null) return;
		carried.Drop();

		if(carried.Mass() <= settings.strength)
			skeleton.Drop(Clothes.ESlot.RIGHT_HAND);

		animator.SetBool("Holding", false);
		animator.SetBool("Carrying", false);

		carried = null;
	}

	public void AddToStat(EStat name, float amount)
	{
		stats[name] += amount;
		if(stats[name] >= 100f) stats[name] = 100f;
		else if(stats[name] < 0f) stats[name] = 0f;
	}
	public void SetStat(EStat name, float value)
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
		collider.enabled = false;
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
		collider.enabled = true;
		agent.enabled = true;
		animator.SetBool("Sitting", false);
		animator.SetBool("Chairing", false);
	}

#if UNITY_EDITOR
	void OnDrawGizmos()
    {
        if(EditorApplication.isPlaying)
		{
			Handles.Label(transform.position + Vector3.up * 2f, graph.GetState().name);
			

			float h = 0f;
			h+= 0.1f;
			foreach(KeyValuePair<EStat, float> entry in stats)
			{
				Handles.Label(transform.position + Vector3.up * (2f + h), entry.Key.ToString() + " = " + entry.Value.ToString());
				h+= 0.1f;
			}
        }
    }
#endif
}