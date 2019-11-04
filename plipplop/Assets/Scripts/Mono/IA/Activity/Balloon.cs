using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Balloon : Activity
{
    [Header("BALLOON")]
    public float distanceBetween = 3f;
    public float timeBetweenThrows = 2f;

    // SYSTEM
    Vector3 originPosition;
    private int slots = 2;
    private int carrier = 0;
    private float timer;
    private bool[] inPlace;
    private bool playing;

    void Start()
    {
        originPosition = transform.position;
    }

    public override void Kick(NonPlayableCharacter user)
    {
        if(user == users[carrier]) user.Drop();
        
        base.Kick(user);

        if(users.Count > 0) 
        {
            Next();
            users[carrier].Carry(transform);
        }

        Initialize();
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

        if(!full)
        {
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
        else if(playing)
        {
            if(timer > 0f) timer -= Time.deltaTime;
            else
            {
                LookAtEachOthers();
                users[carrier].Drop();
                Next();
                users[carrier].Carry(transform);

                timer = timeBetweenThrows;
            }
        }
    }

    void LookAtEachOthers()
    {
        if(users.Count > 0)
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
