using UnityEngine;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class Bird : MonoBehaviour
{
	public enum State { MOVING, FLYING, LANDED };
	public enum WingPosition { UP, MIDDLE, DOWN };

	public class NearObject
	{
		public Transform transform;
		public Vector3 position;
		public Vector3 velocity;
		public float magnitude;

		public NearObject(Transform t)
		{
			this.transform = t;
			this.position = t.position;
		}

		public void Update()
		{
			if (transform == null) return;

			velocity = transform.position - position;
			magnitude = velocity.magnitude;
			position = transform.position;
		}
	}
	void RemoveNearObject(Transform t)
	{
		foreach(NearObject no in nearObjects)
		{
			if(no.transform == t.transform)
			{
				nearObjects.Remove(no);
				return;
			}
		}
	}

	[Header("References")]
	public SkinnedMeshRenderer smr;
	public SkinnedMeshRenderer idleSMR;
	public CollisionEventTransmitter range;
	public GameObject idleGameObject;
	public GameObject flyingGameObject;
	public Transform visuals;

	[Header("Settings")]
	public float refreshTime = 0.25f;
	public float speed = 2f;
	public float treshold = 0.5f;
	public float angleMax = 45f;
	public float fearObjectMagnitude = 0.1f;
	public Vector2 sizeRange;

	public System.Action onReached;
	public System.Action storedOnReached;
	private Vector3 target;
	private bool going;
	private WingPosition currentWingState = WingPosition.MIDDLE;
	private WingPosition previousWingState = WingPosition.DOWN;
	private bool lookingRight = false;
	private float refreshTimer = 0f;
	private Vector3 position;
	private Vector3 positionLast;
	private Vector3 positionDelta;
	private Vector3 rotation;
	private Vector3 rotationLast;
	private Vector3 rotationDelta;
	private float timeOffset;
	private State state;
	private bool faceDirection = false;
	private NearObject objectOn = null;
	private bool unfearable;

	private List<NearObject> nearObjects = new List<NearObject>();

	void Start()
    {
		visuals.transform.localScale = Vector3.one * Random.Range(sizeRange.x, sizeRange.y);
		timeOffset = Random.Range(-1f, 1f);
		position = transform.position;
		rotation = transform.rotation.eulerAngles;

		range.onTriggerEnter += (other) =>
		{
			nearObjects.Add(new NearObject(other.transform));
		};
		range.onTriggerExit += (other) =>
		{
			RemoveNearObject(other.transform);
		};

		FlyOff();
	}

	public void ScaredFrom(Vector3 pos)
	{
		Vector3 hPos = new Vector3(pos.x, transform.position.y, pos.z);
		Vector3 dir = (transform.position - hPos).normalized;
		dir.y = 0.5f;

		StopAllCoroutines();
		MoveTo(transform.position + dir * 3f, () => { FlyOff(); });
	}

	public void FlyOff()
	{
		StopAllCoroutines();
		BirdPath bp = Game.i.aiZone.GetRandomBirdPath();
		if (bp != null)
		{
			Follow(bp);
			StartCoroutine(WaitBeforeIdle(Random.Range(10f, 20f)));
		}
		else
		{
			Debug.LogWarning("No Bird Path found in this scene");
			Destroy(gameObject);
		}
	}

	[ContextMenu("GoSit")]
	public void GoSitOnSpot()
	{
		StopAllCoroutines();
		BirdArea area = Game.i.aiZone.GetRandomBirdArea();
		if (area != null)
		{
			onReached = null;
			BirdArea.Spot s = area.GetSpot();
			SitOn(s.position, s.surface);
		}
		else
		{
			Debug.LogWarning("No Bird Area found in this scene");
			Destroy(gameObject);
		}
	}

	IEnumerator WaitBeforeSit(float time)
	{
		yield return new WaitForSeconds(time);
		GoSitOnSpot();
	}
	IEnumerator WaitBeforeFlyingOff(float time)
	{
		yield return new WaitForSeconds(time);
		FlyOff();
	}
	IEnumerator WaitBeforeIdle(float time)
	{
		yield return new WaitForSeconds(time);
		Idle();
	}

	public void SitOn(Vector3 pos, Transform tran)
	{
		objectOn = new NearObject(tran);

		MoveTo(pos, () =>
		{
			position = pos;
			rotation.y = Random.Range(0f, 360f);
			EnterState(State.LANDED);
		});
	}

	private BirdPath path = null;
	private int pointIndex = 0;
	private int initialPointIndex = 0;
	public void Follow(BirdPath _path)
	{
		path = _path;
		pointIndex = Random.Range(0, path.points.Count);
		initialPointIndex = pointIndex;
		GoToNextPoint();
	}

	public void GoToNextPoint()
	{
		if (path == null) return;

		pointIndex++;
		if (pointIndex >= path.points.Count) pointIndex = 0;

		MoveTo(path.GetPosition(pointIndex), () =>
		{
			onReached = null;
			this.GoToNextPoint();
		});
		/*
		if (pointIndex != initialPointIndex)
		{
			MoveTo(path.GetPosition(pointIndex), () =>
			{
				onReached = null;
				this.GoToNextPoint();
			});
		}
		else
		{
			GoSitOnSpot();
		}
		*/
	}

	public void Stop()
	{
		onReached = null;
		position = transform.position;
	}

	public void Idle()
	{
		EnterState(Bird.State.FLYING);
	}

	public void MoveTo(Vector3 pos, System.Action onReached = null)
	{
		EnterState(State.MOVING);
		going = true;
		target = pos;
		this.onReached += onReached;
	}

	void EnterState(State nState)
	{
		ExitState(state);
		state = nState;
		switch (state)
		{
			case Bird.State.MOVING:
				flyingGameObject.SetActive(true);
				faceDirection = true;
				break;
			case Bird.State.FLYING:
				Stop();
				transform.forward = Vector3.forward;
				visuals.localEulerAngles = Vector3.one;

				smr.SetBlendShapeWeight(0, 0f);
				smr.SetBlendShapeWeight(1, 0f);
				flyingGameObject.SetActive(true);
				StartCoroutine(WaitBeforeSit(Random.Range(10f, 20f)));
				break;
			case Bird.State.LANDED:
				idleGameObject.SetActive(true);
				StartCoroutine(WaitBeforeFlyingOff(Random.Range(10f, 20f)));
				break;
		}
	}

	void ExitState(State nState)
	{
		switch (nState)
		{
			case Bird.State.MOVING:
				flyingGameObject.SetActive(false);
				faceDirection = false;
				break;
			case Bird.State.FLYING:
				flyingGameObject.SetActive(false);
				break;
			case Bird.State.LANDED:
				idleGameObject.SetActive(false);
				break;
		}
	}

	void UpdateFrameState()
	{
		switch (state)
		{
			case Bird.State.MOVING:
				Flap();
				break;
			case Bird.State.FLYING:
				//Flap();
				break;
			case Bird.State.LANDED:
				if (Random.Range(0f, 1f) >= 0.9f) LookOtherWay();
				break;
		}
	}

	void UpdateState()
	{
		switch(state)
		{
			case Bird.State.MOVING:
				break;
			case Bird.State.FLYING:
				position += (transform.right * Mathf.Sin(Time.time + timeOffset) + transform.up * Mathf.Cos(Time.time + timeOffset)) * 0.01f;
				position += transform.right * Mathf.Sin(Time.time + timeOffset) * 0.005f;
				break;
			case Bird.State.LANDED:
				if(!unfearable)
				{
					foreach (NearObject no in nearObjects)
					{
						no.Update();
						if (no.magnitude > fearObjectMagnitude)
						{
							ScaredFrom(no.transform.position);
						}
					}
					if (objectOn != null)
					{
						objectOn.Update();
						if (objectOn.magnitude > 0) FlyOff();
					}
				}
				break;
		}
	}

	void Frame()
	{
		transform.position = position;
		transform.eulerAngles = rotation;

		positionDelta = transform.position - positionLast;
		positionLast = transform.position;
		rotationDelta = transform.eulerAngles - rotationLast;
		rotationLast = transform.eulerAngles;

		if(faceDirection)
		{
			visuals.transform.forward = -positionDelta.normalized;
		}
		else
		{
			visuals.localEulerAngles = positionDelta.normalized * angleMax;
		}
	}

	void Flap()
	{
		WingPosition _previous = currentWingState;

		if(currentWingState == WingPosition.MIDDLE)
		{
			if(previousWingState == WingPosition.DOWN)
			{
				currentWingState = WingPosition.UP;
				smr.SetBlendShapeWeight(0, 0f);
				smr.SetBlendShapeWeight(1, 100f);
			}
			else
			{
				currentWingState = WingPosition.DOWN;
				smr.SetBlendShapeWeight(0, 100f);
				smr.SetBlendShapeWeight(1, 0f);
			}
		}
		else
		{
			currentWingState = WingPosition.MIDDLE;
			smr.SetBlendShapeWeight(0, 0f);
			smr.SetBlendShapeWeight(1, 0f);
		}

		previousWingState = _previous;
	}

	void LookOtherWay()
	{
		if (lookingRight)
		{
			lookingRight = false;
			idleSMR.SetBlendShapeWeight(0, 0f);

			if (Random.Range(0f, 1f) >= 0.5f) idleSMR.SetBlendShapeWeight(1, 100f);
		}
		else
		{
			lookingRight = true;
			idleSMR.SetBlendShapeWeight(1, 0f);
			if (Random.Range(0f, 1f) >= 0.5f) idleSMR.SetBlendShapeWeight(0, 100f);
		}
	}

    void Update()
    {
		UpdateState();

		if (refreshTimer > 0) refreshTimer -= Time.deltaTime;
		else
		{
			Frame();
			UpdateFrameState();
			refreshTimer = refreshTime;
		}

		if(going)
		{
			position += (target - position).normalized * speed * Time.deltaTime;
			if (Vector3.Distance(position, target) < treshold)
			{
				going = false;
				if (onReached != null)
				{
					storedOnReached += onReached;
					onReached = null;
					storedOnReached.Invoke();
					storedOnReached = null;
				}
			}
		}
	}

#if UNITY_EDITOR
	void OnDrawGizmos()
	{
		Gizmos.color = new Color32(255, 215, 0, 255);
		if (EditorApplication.isPlaying)
		{
			UnityEditor.Handles.DrawLine(transform.position, position);
		}
	}
#endif
}
