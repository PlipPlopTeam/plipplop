using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Balloon : Activity, ICarryable
{
    [Header("BALLOON")]
    public float minDistanceBetween = 3f;
    public float maxDistanceBetween = 5f;
	public float distanceMax = 3f;
    public float timeBetweenThrows = 2f;
    public float verticalForce = 50000f;
    public float horizontalForce = 25000f;

	private Vector3 originPosition;
    private int carrier = 0;
    private float throwTimer;
    private List<bool> inPlace = new List<bool>();
    private bool playing;
    private bool flying;
	private Rigidbody rb;
	private Collider col;

	private bool carried = false;
	public bool IsCarried() { return carried; }

	public override void Break()
	{
		base.Break();
		if(users.Count > 0) users[carrier].Drop();
		Initialize();
	}

	void Awake()
	{
		rb = GetComponent<Rigidbody>();
		col = GetComponent<Collider>();
	}
	public virtual void Carry()
    {
        if(col != null) col.enabled = false;
        if(rb != null) rb.isKinematic = true;
		carried = true;
    }
    public virtual void Drop()
    {
        if(col != null) col.enabled = true;
        if(rb != null) rb.isKinematic = false;
		carried = false;
	}
    public float Mass()
    {
        if(rb == null) return 0;
        MeshFilter[] meshFilters = gameObject.GetComponentsInChildren<MeshFilter>();
        Vector3 size = Vector3.one;
        foreach(MeshFilter mf in meshFilters)
        {
            if(mf.mesh.bounds.size.magnitude > size.magnitude)
                size = mf.mesh.bounds.size;
        }
        return (transform.localScale.magnitude * 3f) * size.magnitude * rb.mass;
    }
    public Transform Self()
    {
        return transform;
    }

    void Start()
    {
        originPosition = transform.position;
    }

	public override void StopUsing(NonPlayableCharacter user)
	{

		if (user.IsCarrying(this)) user.Drop();
		if (user.look != null) user.look.LooseFocus();

		base.StopUsing(user);

		if (users.Count > 1)
		{
			carrier = Next();
			users[carrier].Collect(this);
		}
		else Initialize();
	}

	public override void Enter(NonPlayableCharacter user)
    {
        base.Enter(user);
        user.look.FocusOn(transform);
	}

	public override void StartUsing(NonPlayableCharacter user)
	{
		base.StartUsing(user);
		if (users.Count >= 2) GetInPlace();
		else users[carrier].Collect(this);
	}

	bool GoodPositions()
	{
		float distance = 0f;
		for(int i = 0; i < users.Count - 1; i++) distance += Vector3.Distance(users[i].transform.position, users[i + 1].transform.position);
		return distance > distanceMax * users.Count;
	}

    void GetInPlace()
    {
        playing = false;
        flying = false;
        inPlace.Clear();

		float distance = Random.Range(minDistanceBetween, maxDistanceBetween);
		int count = 0;
		foreach (NonPlayableCharacter user in users)
		{
			int spot = inPlace.Count;
			inPlace.Add(false);
			float angle = ((Mathf.PI * 2f) / users.Count) * count;
			Vector3 pos = new Vector3(Mathf.Cos(angle) * distance, 0f, Mathf.Sin(angle) * distance);
			user.agentMovement.GoThere(originPosition + pos);
			user.agentMovement.onDestinationReached += () =>
			{
				inPlace[spot] = true;
				IsAllInPlace();
			};
			count++;
		}
    }

    void Initialize()
    {
        full = false;
        playing = false;
        carrier = 0;
    }

    public override void Update()
    {
        base.Update();
        
        if(playing)
        {
            if(throwTimer > 0f) throwTimer -= Time.deltaTime;
            else
            {
                if(!flying)
                {
					if (GoodPositions())
					{
						int next = Next();
						LookAtEachOthers();
						users[carrier].Drop();
						// Throwing
						users[carrier].transform.forward = -(users[carrier].transform.position - users[next].transform.position).normalized;
						transform.position += users[carrier].transform.forward * 0.5f;
						Vector3 throwVector = users[carrier].transform.forward;
						rb.AddForce(new Vector3(throwVector.x, 0f, throwVector.z) * horizontalForce * Time.deltaTime);
						rb.AddForce(new Vector3(0f, 1f, 0f) * verticalForce * Time.deltaTime);
						carrier = next;
						users[carrier].Collect(this);
						flying = true;
					}
					else GetInPlace();
				}
                else
                {
                    if(users[carrier].IsCarrying(this))
                    {
                        LookAtEachOthers();
                        users[carrier].agentMovement.Stop();
                        throwTimer = timeBetweenThrows;
                        flying = false;
                    }
                }
            }
        }
    }

    void LookAtEachOthers()
    {
		List<Vector3> positions = new List<Vector3>();
		foreach (NonPlayableCharacter user in users) positions.Add(user.transform.position);

		Vector3 center = Geometry.CenterOfPoints(positions.ToArray());
		foreach (NonPlayableCharacter user in users) user.transform.forward = (transform.position - center).normalized;
	}

    void IsAllInPlace()
    {
		foreach(bool b in inPlace) if(!b) return;
		if (!users[carrier].IsCarrying(this)) return;
        playing = true;
        LookAtEachOthers();
    }

    bool IsAllTrue(bool[] array)
    {
        foreach(bool b in array) if(!b) return false;
        return true;
    }

    int Next()
    {
		int newCarrier = carrier + 1;
        if(newCarrier >= users.Count) newCarrier = 0;
		return newCarrier;
    }
}
