using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jukebox : Activity
{
    public override void Enter(NonPlayableCharacter user)
    {
        base.Enter(user);
        user.agentMovement.Stop();
        user.animator.SetBool("Dancing", true);
    }

    public override void Exit(NonPlayableCharacter user)
    {
        base.Exit(user);
        user.animator.SetBool("Dancing", false);
    }
}
