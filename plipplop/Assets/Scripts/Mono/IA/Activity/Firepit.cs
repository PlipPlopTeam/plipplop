using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Firepit : Activity
{
    [Header("FIREPIT")]
    public float radius = 2f;


    public override void Enter(NonPlayableCharacter user)
    {
        base.Enter(user);

        float angle = Random.Range(0f, 1f) * Mathf.PI * 2;
        Vector3 pos = new Vector3(transform.position.x + Mathf.Cos(angle) * radius, transform.position.y, transform.position.z + Mathf.Sin(angle) * radius);

        user.agentMovement.GoThere(pos);
        user.agentMovement.onDestinationReached += () =>
        {
            user.agentMovement.Stop();
            user.animator.SetBool("Sitting", true);
            user.transform.LookAt(transform.position);
        };
    }

    public override void Exit(NonPlayableCharacter user)
    {
        base.Exit(user);
        user.animator.SetBool("Sitting", false);
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color32(255, 215, 0, 255);
        UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.up, radius);
    }
#endif
}
