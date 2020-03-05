using System.Collections;
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
	public enum ESubject { PLAYER, VALUABLE, ACTIVITY, CHAIR, FOOD, FEEDER, CHARACTER };

	[Header("References")]
	[HideInInspector] public Sight sight;
	[HideInInspector] public FocusLook look;
	[HideInInspector] public NavMeshAgent agent;
	[HideInInspector] public AgentMovement agentMovement;
	[HideInInspector] public Animator animator;
	[HideInInspector] public Skeleton skeleton;
	[HideInInspector] public EmotionRenderer emo;
	[HideInInspector] public Range range;
	[HideInInspector] public Face face;
	[HideInInspector] public Collider collider;
	public SkinnedMeshRenderer skin;
	[HideInInspector] public ICarryable carried;
	[Header("Read-Only")]
	public Controller player;
	public Controller rPlayer;
	public NonPlayableCharacter character;
	public Valuable valuable;
	public Activity activity;
	public Activity previousActivity;
	public Chair chair;
	public Food food;
    public Feeder feeder;

	public Dictionary<Cloth.ESlot, Cloth> clothes = new Dictionary<Cloth.ESlot, Cloth>();
	public Dictionary<EStat, float> stats = new Dictionary<EStat, float>();

	[Header("Settings")]
	public NonPlayableCharacterSettings settings;
	public BehaviorGraphData graph;
    ICarryable carryableToCollect;
	private float waitTimer;
	private bool endWait;
	[HideInInspector] public bool hasWaited;
	public System.Action onWaitEnded;
	public System.Action onCollect;

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

		// Load Character Cloth Slots
		foreach (Cloth.ESlot suit in (Cloth.ESlot[])Cloth.ESlot.GetValues(typeof(Cloth.ESlot)))
			clothes.Add(suit, null);
	}

	public void Start()
	{
		// Loading Settings
		if (settings == null) settings = Game.i.library.npcLibrary.defaultSettings;
		settings = Instantiate(settings);
		settings.Load();
		skeleton.gameObject.transform.localScale = Vector3.one * settings.height / 2;
		skin.SetBlendShapeWeight(7, settings.GetWeightRatio() * 100f);
		stats.Add(EStat.BOREDOM, settings.initialBoredom);
		stats.Add(EStat.TIREDNESS, settings.initialTiredness);
		stats.Add(EStat.HUNGER, settings.initialTiredness);

		// Loading Graph
		if (graph == null)
		{
			Destroy(gameObject);
			return;
		}
		graph = Instantiate(graph);
		graph.Load(this);

		EquipOutfit();
	}

	public void RememberPlayerFor(float time)
	{		
		rPlayer = player;
		StartCoroutine(WaitAndStopExposing(time));
	}
	IEnumerator WaitAndForgetRememberedPlayer(float time)
	{
		yield return new WaitForSeconds(time);
		rPlayer = null;
	}

	// SHOW OFF
	private Activity show;
	public void ShowOff(float time, Vector2 range, int slot)
	{
		show = gameObject.AddComponent<Activity>();
		show.spectatorMax = slot;
		show.full = true;
		show.working = true;
		show.spectatorRange = range;
		StartCoroutine(WaitAndStopExposing(time));
	}
	IEnumerator WaitAndStopExposing(float time)
	{
		yield return new WaitForSeconds(time);
		if(show != null)
		{
			show.Dismantle();
			show = null;
		}
	}

	public void EquipOutfit()
	{
		if (settings.autoOutfit)
		{
			foreach (ClothData c in Game.i.library.GetOutfit()) Equip(c);
		}
		else
		{
			foreach (ClothData c in settings.clothes) Equip(c);
		}
	}

	public virtual void Update()
	{
		UpdateCollecting();
		UpdateWaiting();
		agentMovement.Tick();
		if (carried != null)
		{
			if (carried.Mass() > settings.strength) Lift(carried);
			else Carrying(carried);
		}
		graph.Update();
	}

	public virtual void Carrying(ICarryable carryable)
	{}

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

	public void Equip(ClothData clothData, bool change = true)
	{
		// CHECK IF THIS CLOTH IS BANNED
		foreach (KeyValuePair<Cloth.ESlot, Cloth> key in clothes)
		{
			if(key.Value != null)
			{
				foreach (Cloth.ESlot s in key.Value.data.bannedSlot)
				{
					if (s == clothData.slot) return;
				}
			}
		}

		Cloth c = clothes[clothData.slot];
		if(c != null && change) c.Destroy();

		GameObject clothObject = new GameObject();
		clothObject.name = clothData.name;
		c = clothObject.AddComponent<Cloth>();
		c.Create(clothData, skeleton);
		c.SetWeight(settings.GetWeightRatio() * 100f);

		// UNEQUIP BANNED CLOTHES
		foreach (Cloth.ESlot s in c.data.bannedSlot)
		{
			if(clothes[clothData.slot] != null)
				clothes[clothData.slot].Destroy();
		}

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

	public void Lift(ICarryable carryable)
	{
		carryable.Self().position = (skeleton.GetSocketBySlot(Cloth.ESlot.RIGHT_HAND).GetPosition() + skeleton.GetSocketBySlot(Cloth.ESlot.LEFT_HAND).GetPosition())/2f;
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
				if(onCollect != null)
				{
					onCollect.Invoke();
					onCollect = null;
				}
			}
		}
	}

	public bool IsCollecting(ICarryable carryable)
	{
		return carryable == carryableToCollect;
	}
	public void StopCollecting()
	{
		carryableToCollect = null;
		agentMovement.StopChase();
	}
	public void Collect(ICarryable carryable, System.Action then = null)
	{
		carryableToCollect = carryable;
		agentMovement.Chase(carryableToCollect.Self());

		if (then != null) onCollect += then;
	}

	public void Consume(Food f)
	{
		food = f;
		agentMovement.Stop();
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
			if(animator != null) animator.SetBool("Holding", true);
			if(skeleton != null) skeleton.Attach(carried.Self(), Cloth.ESlot.RIGHT_HAND, true);
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
			skeleton.Drop(Cloth.ESlot.RIGHT_HAND);

		if(animator != null)
		{
			animator.SetBool("Holding", false);
			animator.SetBool("Carrying", false);
		}
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
		agentMovement.onDestinationReached += () => {Sit();};
	}

	public void GoSitThere(Chair chair, Chair.Spot spot)
	{
		agentMovement.GoThere(chair.transform.position + spot.position);
		agentMovement.onDestinationReached += () =>
		{
			Sit(chair);
			chair.Sit(this, spot);
		};
	}

	public void Sit()
	{
		agentMovement.Stop();
		agent.enabled = false;
		animator.SetBool("Sitting", true);
	}

	public void Sit(Vector3 pos)
	{
		agentMovement.Stop();
		agent.enabled = false;
		transform.position = pos;
		animator.SetBool("Sitting", true);
	}
	public void Sit(Chair c)
	{
		collider.enabled = false;
		chair = c;
		agentMovement.Stop();
		agent.enabled = false;
		transform.SetParent(c.transform);
		transform.localPosition = Vector3.zero;
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
			GUIStyle s = new GUIStyle();
			s.alignment = TextAnchor.MiddleCenter;
			s.fontStyle = FontStyle.Bold;
			s.normal.textColor = Color.white;
			Handles.Label(transform.position + Vector3.up * 2f, graph.GetState().name, s);
			/*
			float h = 0f;
			h+= 0.1f;
			foreach(KeyValuePair<EStat, float> entry in stats)
			{
				Handles.Label(transform.position + Vector3.up * (2f + h), entry.Key.ToString() + " = " + entry.Value.ToString());
				h+= 0.1f;
			}
			*/
        }
    }
#endif
}