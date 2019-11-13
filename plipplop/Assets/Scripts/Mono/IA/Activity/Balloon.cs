using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Balloon : Activity
{
    [Header("BALLOON")]
    public float distanceBetween = 3f;
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

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        originPosition = transform.position;
    }

    public override void Exit(NonPlayableCharacter user)
    {
        if(user == users[carrier]) user.Drop();

        user.look.LooseFocus();
        
        base.Exit(user);

        if(users.Count > 0) 
        {
            Next();
            users[carrier].Carry(transform);
        }

        Initialize();
    }

    public override void Enter(NonPlayableCharacter user)
    {
        base.Enter(user);
        user.look.FocusOn(transform);
        if(users.Count >= slots)
        {
            inPlace = new bool[slots];
            full = true;
            users[0].Carry(transform);
            users[0].agentMovement.GoThere(originPosition + Vector3.forward * distanceBetween/2);
            users[0].agentMovement.onDestinationReached += () =>
            {
                inPlace[0] = true;
                IsAllInPlace();
            };

            users[1].agentMovement.GoThere(originPosition + Vector3.forward * -distanceBetween/2);
            users[1].agentMovement.onDestinationReached += () =>
            {
                inPlace[1] = true;
                IsAllInPlace();
            };
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
                    LookAtEachOthers();
                    users[carrier].Drop();

                    // Throwing
                    Vector3 throwVector = users[carrier].transform.forward;
                    rb.AddForce(new Vector3(throwVector.x, 0f, throwVector.z)  * horizontalForce * Time.deltaTime);
                    rb.AddForce(new Vector3(0f, 1f, 0f)  * verticalForce * Time.deltaTime);

                    Next();
                    users[carrier].Collect(transform);

                    flying = true;
                }
                else
                {
                    if(users[carrier].IsCarrying(transform))
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
