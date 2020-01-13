using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Balloon : Activity, ICarryable
{
    [Header("BALLOON")]
    public float minDistanceBetween = 3f;
    public float maxDistanceBetween = 5f;
    public float timeBetweenThrows = 2f;
    public float verticalForce = 50000f;
    public float horizontalForce = 25000f;

    // SYSTEM
    Vector3 originPosition;
    private int slots = 2;
    private int carrier = 0;
    private float throwTimer;
    private bool[] inPlace;
    private bool playing;
    private bool flying;

    private Rigidbody rb;
    private Collider col;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
    }

    public virtual void Carry()
    {
        if(col != null) col.enabled = false;
        if(rb != null) rb.isKinematic = true;
    }

    public virtual void Drop()
    {
        if(col != null) col.enabled = true;
        if(rb != null) rb.isKinematic = false;
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

    public override void Exit(NonPlayableCharacter user)
    {
        if(carrier < users.Count && user == users[carrier]) user.Drop();

        user.look.LooseFocus();
        
        base.Exit(user);

        if(users.Count > 0) 
        {
            Next();
            users[carrier].Carry(this);
        }

        Initialize();
    }

    float GetRandomDistance()
    {
        return Random.Range(minDistanceBetween, maxDistanceBetween);
    }

    public override void Enter(NonPlayableCharacter user)
    {
        base.Enter(user);
        user.look.FocusOn(transform);
        if(users.Count >= slots)
        {
            full = true;
            users[carrier].Carry(this);
            GetInPlace();
        }
    }

    void GetInPlace()
    {
        playing = false;
        flying = false;
        inPlace = new bool[slots];
		users[0].agentMovement.Stop();
		users[0].agentMovement.GoThere(originPosition + Vector3.forward * GetRandomDistance()/2);
		users[0].agentMovement.onDestinationReached += () =>
        {
            inPlace[0] = true;
            IsAllInPlace();
        };
		users[1].agentMovement.Stop();
		users[1].agentMovement.GoThere(originPosition + Vector3.forward * -GetRandomDistance()/2);
		users[1].agentMovement.onDestinationReached += () =>
        {
            inPlace[1] = true;
            IsAllInPlace();
        };
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
                    if(DistanceBetweenPlayers() > minDistanceBetween
                    && DistanceBetweenPlayers() < maxDistanceBetween)
                    {
                        LookAtEachOthers();
                        users[carrier].Drop();

                        // Throwing
                        transform.position += users[carrier].transform.forward * 0.5f;
                        Vector3 throwVector = users[carrier].transform.forward;
                        rb.AddForce(new Vector3(throwVector.x, 0f, throwVector.z)  * horizontalForce * Time.deltaTime);
                        rb.AddForce(new Vector3(0f, 1f, 0f)  * verticalForce * Time.deltaTime);

                        Next();
                        users[carrier].Collect(this);

                        flying = true;
                    }
                    else 
                    {
                        GetInPlace();
                    }
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

    float DistanceBetweenPlayers()
    {
        return Vector3.Distance(users[0].transform.position, users[1].transform.position);
    }

    void LookAtEachOthers()
    {
        if(users.Count >= 2)
        {
            users[0].transform.LookAt(users[1].transform);
            users[1].transform.LookAt(users[0].transform);
        }
    }

    void IsAllInPlace()
    {
        if(!IsAllTrue(inPlace)) return;
        
        playing = true;
        LookAtEachOthers();
    }

    bool IsAllTrue(bool[] array)
    {
        foreach(bool b in array) if(!b) return false;
        return true;
    }

    void Next()
    {
        carrier++;
        if(carrier >= users.Count) carrier = 0;
    }
}
