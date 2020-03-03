using UnityEngine;
#if UNITY_EDITOR
	using UnityEditor;
#endif

public class Bird : MonoBehaviour
{
	public enum State { MOVING, FLYING, LANDED };

	[Header("References")]
	public SkinnedMeshRenderer smr;
	public SkinnedMeshRenderer idleSMR;
	public GameObject idleGameObject;
	public GameObject flyingGameObject;
	public Transform visuals;

	[Header("Settings")]
	public float refreshTime = 0.25f;
	public float speed = 2f;
	public float treshold = 0.5f;
	public float angleMax = 45f;
	public Vector2 sizeRange;

	public System.Action onDestinationReached;
	private Vector3 target;
	private bool going;
	private bool wingUp = false;
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

	void Start()
    {
		visuals.transform.localScale = Vector3.one * Random.Range(sizeRange.x, sizeRange.y);
		timeOffset = Random.Range(-1f, 1f);

		position = transform.position;
		rotation = transform.rotation.eulerAngles;

		EnterState(Bird.State.FLYING);

		BirdPath bp = FindObjectOfType<BirdPath>();
		if(bp != null) Follow(bp);
	}

	[ContextMenu("GoSit")]
	public void GoToSpot()
	{
		BirdArea ba = FindObjectOfType<BirdArea>();
		if(ba != null)
		{
			Vector3 pos = ba.GetLandPosition();
			MoveTo(pos, () =>
			{
				position = pos;
				rotation.y = Random.Range(0f, 360f);
				EnterState(State.LANDED);
			});
		}
	}

	void ExitState(State nState)
	{
		switch (nState)
		{
			case Bird.State.MOVING:
				flyingGameObject.SetActive(false);
				break;
			case Bird.State.FLYING:
				flyingGameObject.SetActive(false);
				break;
			case Bird.State.LANDED:
				idleGameObject.SetActive(false);
				break;
			default: break;
		}
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

		if(pointIndex != initialPointIndex)
		{
			MoveTo(path.GetPosition(pointIndex), () =>
			{
				this.GoToNextPoint();
				Debug.Log("this.GoToNextPoint()");
			});
		}
		else
		{
			GoToSpot();
		}
	}

	public void MoveTo(Vector3 pos, System.Action onReached = null)
	{
		EnterState(State.MOVING);
		going = true;
		target = pos;
		onDestinationReached += onReached;
	}

	void EnterState(State nState)
	{
		ExitState(state);
		state = nState;
		switch (state)
		{
			case Bird.State.MOVING:
				flyingGameObject.SetActive(true);
				break;
			case Bird.State.FLYING:
				flyingGameObject.SetActive(true);
				break;
			case Bird.State.LANDED:
				idleGameObject.SetActive(true);
				break;
			default: break;
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
			default: break;
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
				break;
			default: break;
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

		visuals.localEulerAngles = positionDelta.normalized * angleMax;
	}

	void Flap()
	{
		if (wingUp)
		{
			wingUp = false;
			smr.SetBlendShapeWeight(0, 100f);
			smr.SetBlendShapeWeight(1, 0f);
		}
		else
		{
			wingUp = true;
			smr.SetBlendShapeWeight(0, 0f);
			smr.SetBlendShapeWeight(1, 100f);
		}
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
				if (onDestinationReached != null)
				{
					onDestinationReached.Invoke();
					onDestinationReached = null;
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
