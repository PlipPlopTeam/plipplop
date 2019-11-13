using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Firepit : Activity
{
    [Header("FIREPIT")]
    public Chair[] chairs;
    public float radius = 2f;

    public override void Enter(NonPlayableCharacter user)
    {
        base.Enter(user);
        Chair chair = null;
        foreach(Chair c in chairs)
        {
            if(!c.IsFull())
            {
                chair = c;
                break;
            }
        }

        foreach(NonPlayableCharacter u in users)
        {
            NonPlayableCharacter npc = users[Random.Range(0, users.Count)];
            if(npc != user) u.look.FocusOn(npc.skeleton.headBone.transform);
        }

        if(chair == null)
        {
            float angle = Random.Range(0f, 1f) * Mathf.PI * 2;
            Vector3 pos = new Vector3(transform.position.x + Mathf.Cos(angle) * radius, transform.position.y, transform.position.z + Mathf.Sin(angle) * radius);

            user.GoSitThere(pos);
            user.agentMovement.onDestinationReached += () =>
            {
                user.transform.LookAt(transform.position);
            };
        }
        else chair.Enter(user);
    }

    public override void Exit(NonPlayableCharacter user)
    {
        user.GetUp();
        user.look.LooseFocus();
        base.Exit(user);
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color32(255, 215, 0, 255);
        UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.up, radius);
    }
#endif
}
