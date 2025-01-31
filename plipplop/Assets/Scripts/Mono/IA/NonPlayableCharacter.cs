﻿using System.Collections;
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
	[HideInInspector] public AgentMovement movement;
	[HideInInspector] public Animator animator;
	[HideInInspector] public Skeleton skeleton;
	[HideInInspector] public EmotionRenderer emo;
	[HideInInspector] public Range range;
	[HideInInspector] public Face face;
	[HideInInspector] new public Collider collider;
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
	public Container container;

	public Dictionary<Cloth.ESlot, Cloth> clothes = new Dictionary<Cloth.ESlot, Cloth>();
	public Dictionary<EStat, float> stats = new Dictionary<EStat, float>();

	[Header("Settings")]
	public NonPlayableCharacterSettings settings;
	public BehaviorGraphData graph;
	public AIPath assignedPath = null;
    ICarryable carryableToCollect;
	private float waitTimer;
	private bool endWait;
	[HideInInspector] public Vector3 spawnPosition;
	[HideInInspector] public bool hasWaited;
	public System.Action onWaitEnded;
	public System.Action onCollect;
	public System.Action onCollectFailed;

	public void Awake()
	{
		skeleton = GetComponentInChildren<Skeleton>();
		sight = GetComponent<Sight>();
		look = GetComponent<FocusLook>();
		agent = GetComponent<NavMeshAgent>();
		animator = GetComponentInChildren<Animator>();
		range = GetComponent<Range>();
		emo = GetComponent<EmotionRenderer>();
		emo.Initialize();
		movement = GetComponent<AgentMovement>();
		movement.animator = animator;
		face = GetComponent<Face>();
		collider = GetComponent<Collider>();

		// Load Character Cloth Slots
		foreach (Cloth.ESlot suit in (Cloth.ESlot[])Cloth.ESlot.GetValues(typeof(Cloth.ESlot)))
			clothes.Add(suit, null);

		spawnPosition = transform.position;
	}

	public void OnDisable()
	{
		HardReset();
	}
	public void OnEnable()
	{
		HardReset();
	}

	public void HardReset()
	{
		ResetAnimator();
		if (activity != null)
		{
			activity.Exit(this);
			activity = null;
		}
		if (chair != null)
		{
			GetUp();
			chair = null;
		}
		feeder = null;
	}


	public void ResetAnimator()
	{
		animator.SetBool("Carrying", false);
		animator.SetBool("Fishing", false);
		animator.SetBool("Sitting", false);
		animator.SetBool("Chairing", false);
		animator.SetBool("Dancing", false);
		animator.SetBool("Consuming", false);
		animator.SetBool("Holding", false);
		animator.SetBool("Scared", false);
		animator.SetBool("Tanning", false);
		animator.SetBool("Guitaring", false);
		animator.SetBool("Crafting", false);
	}

	public void Set(NonPlayableCharacterSettings s)
	{
		settings = Instantiate(s);
		settings.Load();
		skeleton.gameObject.transform.localScale = Vector3.one * settings.height / 2;

		if (skin.sharedMesh.blendShapeCount > 6)
		{
			skin.SetBlendShapeWeight(7, settings.GetWeightRatio() * 100f);
		}

		stats.Add(EStat.BOREDOM, 0f);
		stats.Add(EStat.TIREDNESS, 0f);
		stats.Add(EStat.HUNGER, 0f);
	}

	public void Start()
	{
		if (settings == null) settings = Game.i.library.npcLibrary.defaultSettings;
		Set(settings);

		if(settings.autoOutfit) RandomOutfit();
		else Outfit(settings.clothes);

		// Loading Graph
		if (graph == null)
		{
			Destroy(gameObject);
			return;
		}
		graph = Instantiate(graph);
		graph.Load(this);
	}

	public void RememberPlayerFor(float time)
	{		
		rPlayer = player;
		StartCoroutine(WaitAndForgetRememberedPlayer(time));
	}
	IEnumerator WaitAndForgetRememberedPlayer(float time)
	{
		yield return new WaitForSeconds(time);
		rPlayer = null;
	}

	// SHOW OFF
	private Activity show;
	public virtual void ShowOff(float time, Vector2 range, int slot)
	{
		show = gameObject.AddComponent<ShowOffActivity>();
		show.spectatorMax = slot;
		show.full = true;
		show.working = true;
		show.spectatorRange = range;
		WaitAndDo(time, () => {
			if (show != null)
			{
				show.Dismantle();
				show = null;
			}
		});
	}
	public void WaitAndDo(float time, System.Action action)
	{
		StartCoroutine(WaitAndDoThing(time, action));
	}
	IEnumerator WaitAndDoThing(float time, System.Action action)
	{
		yield return new WaitForSeconds(time);
		action.Invoke();
	}

	public void RandomOutfit()
	{
		List<ClothData> outfit = Game.i.library.GetOutfit();
		foreach (ClothData c in outfit) Equip(c);
	}
	public void Outfit(List<ClothData> outfit)
	{
		foreach (ClothData c in outfit) Equip(c);
	}

	public virtual void Update()
	{
		UpdateCollecting();
		UpdateWaiting();
		UpdateStoring();
		movement.Tick();
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

	Container tContainer = null;
	Item itemToStore = null;
	System.Action onStoredItem;
	public void StopStoring()
	{
		tContainer = null;
		itemToStore = null;
	}
	public void Store(Container c, Item item)
	{
		tContainer = c;
		itemToStore = item;
		movement.Chase(c.transform);
	}
	public void UpdateStoring()
	{
		if (tContainer != null && range.IsInRange(tContainer.gameObject))
		{
			Item item = null;
			if (carried.Self().gameObject.TryGetComponent<Item>(out item))
			{
				if (item == itemToStore) Drop();
			}

			movement.StopChase();
			tContainer.Store(itemToStore);
			StopStoring();

			if(onStoredItem != null)
			{
				onStoredItem.Invoke();
				onStoredItem = null;
			}
		}
	}
	public void StoreCarriedItem(Container c)
	{
		if(carried != null && carried.Self().gameObject.TryGetComponent<Item>(out Item item))
		{
			Store(c, item);
		}
	}

	public void Lift(ICarryable carryable)
	{
		carryable.Self().position = (skeleton.GetSocketBySlot(Cloth.ESlot.RIGHT_HAND).GetPosition() + skeleton.GetSocketBySlot(Cloth.ESlot.LEFT_HAND).GetPosition())/2f;
		carryable.Self().forward = transform.forward;
	}

	int stun = 0;
	public void Stun(float duration = 1f)
	{
		stun++;
		RefreshStun();
		Game.i.WaitAndDo(duration, () => Unstun());
	}
	public void Unstun()
	{
		stun--;
		RefreshStun();
	}
	public void RefreshStun()
	{
		if(stun > 0)
		{
			graph.Pause();
			movement.Pause();
			look.LooseFocus();
			look.FocusOn(transform.position + transform.forward * 1f + transform.up * 1f);
			Pyromancer.PlayVFXAttached("vfx_stun", skeleton.GetSocketBySlot(Cloth.ESlot.HEAD).bone, new Vector3(0f, 0.5f, 0f));
		}
		else
		{
			graph.Play();
			look.LooseFocus();
			movement.Resume();
		}
	}

	public void AutoCollect()
	{
		movement.StopChase();
		Carry(carryableToCollect);
		carryableToCollect = null;
		if (onCollect != null)
		{
			onCollect.Invoke();
			onCollect = null;
		}
	}

	public void UpdateCollecting()
	{
		if(carryableToCollect != null)
		{
			if (carryableToCollect.Self() != null)
			{
				if (range.IsInRange(carryableToCollect.Self().gameObject))
				{
					AutoCollect();
				}
			}
			else AutoCollect();
		}
	}

	public bool IsCollecting(ICarryable carryable)
	{
		return carryable == carryableToCollect;
	}
	public void StopCollecting()
	{
		carryableToCollect = null;
		movement.StopChase();
	}
	public void CollectFailed()
	{
		StopCollecting();
		if (onCollectFailed != null)
		{
			onCollectFailed.Invoke();
			onCollectFailed = null;
		}
	}

	public void Collect(ICarryable carryable, System.Action then = null, System.Action failed = null)
	{
		carryableToCollect = carryable;
		movement.Chase(carryableToCollect.Self(), () => CollectFailed());

		if (then != null) onCollect += then;
		if (failed != null) onCollect += then;
	}

	public void Consume(Food f)
	{
		food = f;
		movement.Stop();
		food.Consume(delegate{
			animator.SetBool("Consuming", false);
			if(this.food != null)
			{
				this.AddToStat(NonPlayableCharacter.EStat.HUNGER, -this.food.data.calory);
				this.emo.Show(Emotion.EVerb.SEARCH, "bin");
				Container c = Game.i.aiZone.GetContainerMadeFor(this.food.type);
				if (c != null)
				{
					this.onStoredItem += () => { this.food = null; };
					this.StoreCarriedItem(c);
				}
				else
				{
					Drop();
					this.food.Drop();
					this.food = null;
				}
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

	public virtual void Drop()
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
	public float GetStat(EStat name)
	{
		return stats[name];
	}

	public void GoSitThere(Vector3 where)
	{
		movement.GoThere(where);
		movement.onDestinationReached += () => {Sit();};
	}

	public void GoSitThere(Chair chair, Chair.Spot spot)
	{
		movement.GoThere(chair.transform.position + spot.position);
		movement.onDestinationReached += () =>
		{
			Sit(chair);
			chair.Sit(this, spot);
		};
	}

	public void Sit()
	{
		movement.Stop();
		agent.enabled = false;
		animator.SetBool("Sitting", true);
	}

	public void Sit(Vector3 pos)
	{
		movement.Stop();
		agent.enabled = false;
		transform.position = pos;
		animator.SetBool("Sitting", true);
	}
	public void Sit(Chair c)
	{
		collider.enabled = false;
		chair = c;
		movement.Stop();
		agent.enabled = false;
		transform.SetParent(c.transform);
		transform.localPosition = Vector3.zero;
		animator.SetBool("Chairing", true);
	}
	public void GetUp()
	{
		if(chair != null)
		{
			if(transform != null) transform.SetParent(null);
			chair.Exit(this);
			chair = null;
			ResetLayer();
		}
		collider.enabled = true;
		agent.enabled = true;
		animator.SetBool("Sitting", false);
		animator.SetBool("Chairing", false);
	}

	public void ResetLayer()
	{
		gameObject.layer = 0;
	}
	
#if UNITY_EDITOR
	void OnDrawGizmos()
    {
        if(EditorApplication.isPlaying 
		&& graph != null 
		&& graph.GetState() != null)
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