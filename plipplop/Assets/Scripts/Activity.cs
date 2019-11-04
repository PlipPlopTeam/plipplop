using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Activity : MonoBehaviour
{
    public List<NonPlayableCharacter> users = new List<NonPlayableCharacter>();
    public bool full;
    public bool working = true;

    public virtual void Enter(NonPlayableCharacter user)
    {
        user.activity = this;
        users.Add(user);
    }

    public virtual void Kick(NonPlayableCharacter user)
    {
        user.activity = null;
        users.Remove(user);
    }

    [ContextMenu("Break")]
    public virtual void Break()
    {
        foreach(NonPlayableCharacter user in users.ToArray()) Kick(user);
        working = false;
    }
}
