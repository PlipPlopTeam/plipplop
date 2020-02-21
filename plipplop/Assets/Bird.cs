using UnityEngine;
#if UNITY_EDITOR
	using UnityEditor;
#endif

public class Bird : MonoBehaviour
{
	enum State { MOVING, FLYING, LANDED };

	[Header("References")]
	public SkinnedMeshRenderer smr;
	public GameObject idleGameObject;
	public GameObject flyingGameObject;

	[Header("Settings")]
	public float refreshTime = 0.25f;
	public float range = 1f;

	private State state;
	private bool wingUp = false;
	private float flappingTimer = 0f;
	private float refreshTimer = 0f;

	public Vector3 position;
	private Vector3 positionLast;
	public Vector3 positionDelta;

	public Vector3 rotation;
	private Vector3 rotationLast;
	public Vector3 rotationDelta;

	void Start()
    {
		position = transform.position;
		rotation = transform.rotation.eulerAngles;

		EnterState(Bird.State.FLYING);
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
				break;
			default: break;
		}
	}

	void UpdateState()
	{
		switch(state)
		{
			case Bird.State.MOVING:
				position += Vector3.up * 0.1f * Time.deltaTime;
				break;
			case Bird.State.FLYING:
				position += (transform.right * Mathf.Sin(Time.time) + transform.up * Mathf.Cos(Time.time)) * 0.01f; 
				break;
			case Bird.State.LANDED:
				break;
			default: break;
		}
	}

	void Frame()
	{
		transform.position = position;
		transform.rotation = Quaternion.Euler(rotation);
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

    void Update()
    {
		positionDelta = transform.position - positionLast;
		positionLast = transform.position;
		rotationDelta = transform.eulerAngles - rotationLast;
		rotationLast = transform.eulerAngles;

		UpdateState();
		Debug.Log(positionDelta.normalized);
		rotation = positionDelta.normalized * 90f;

		if (refreshTimer > 0) refreshTimer -= Time.deltaTime;
		else
		{
			Frame();
			UpdateFrameState();
			refreshTimer = refreshTime;
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
