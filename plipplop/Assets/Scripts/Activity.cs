using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Activity : MonoBehaviour
{
    public NonPlayableCharacter user;

    public void Use(NonPlayableCharacter npc)
    {
        user = npc;
        npc.agentMovement.Stop();
    }
}
