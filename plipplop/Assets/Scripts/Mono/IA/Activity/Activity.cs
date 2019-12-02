using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Activity : MonoBehaviour
{
    [Header("ACTIVITY")]
    public bool full = false;
    public bool working = true;
    [Header("Stats Modifiers")]
    public float boredomMultiplier = 1f;
    public float tirednessMultiplier = 0f;
    public float hungerMultiplier = 0f;
    [Space(5)]
    public List<NonPlayableCharacter> users = new List<NonPlayableCharacter>();

    private float timer = 0f;

    public virtual void Enter(NonPlayableCharacter user)
    {
        user.stats[NonPlayableCharacter.EStat.BOREDOM] = 0f;
        user.activity = this;
        users.Add(user);
    }

    public virtual void Exit(NonPlayableCharacter user)
    {
        user.activity = null;
        user.previousActivity = this;
        users.Remove(user);
    }

    public virtual void KickAll()
    {
        foreach(NonPlayableCharacter user in users.ToArray())
            Exit(user);
    }

    [ContextMenu("Break")]
    public virtual void Break()
    {
        KickAll();
        working = false;
    }

    [ContextMenu("Repair")]
    public virtual void Repair()
    {
        working = true;
    }

    private void Start()
    {
        Game.i.aiZone.Register(this);
    }

    public virtual void Update()
    {
        if(timer > 0f) timer -= Time.deltaTime;
        else
        {
            timer = 1f;
            foreach(NonPlayableCharacter user in users.ToArray())
            {
                user.AddToStat(NonPlayableCharacter.EStat.BOREDOM, boredomMultiplier);
                user.AddToStat(NonPlayableCharacter.EStat.TIREDNESS, tirednessMultiplier);
                user.AddToStat(NonPlayableCharacter.EStat.HUNGER, hungerMultiplier);
            }
        }
    }
}
