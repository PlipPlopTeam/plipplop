﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class FishingBox : Activity
{
    [Header("FISHING-BOX")]
    public GameObject fishingPole;
    public Vector3 position;
    public float radius;

    public override void Enter(NonPlayableCharacter user)
    {
        base.Enter(user);
        user.agentMovement.Stop();
        user.agentMovement.GoThere(transform.position);
        user.agentMovement.onDestinationReached += () =>
        {
            user.skeleton.Attach(Instantiate(fishingPole).transform, Clothes.ESlot.RIGHT_HAND, true, Vector3.zero, new Vector3(180f, 0f, 90f));

            Vector3 pos = position + Geometry.GetRandomPointAround(radius);

            user.agentMovement.GoThere(pos);
            StartCoroutine(DelayedSetup(user));
        };
    }
    IEnumerator DelayedSetup(NonPlayableCharacter user)
    {
        yield return new WaitForEndOfFrame();
        user.agentMovement.onDestinationReached += () =>
        {
            user.agentMovement.Stop();
            user.animator.SetBool("Fishing", true);
            user.transform.LookAt(position);
        };
    }

    public override void Exit(NonPlayableCharacter user)
    {
        base.Exit(user);
        user.skeleton.Drop(Clothes.ESlot.RIGHT_HAND);
        user.animator.SetBool("Fishing", false);
    }


#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color32(255, 215, 0, 255);
        UnityEditor.Handles.DrawLine(transform.position, position);
        UnityEditor.Handles.DrawWireDisc(position, Vector3.up, radius);
    }
#endif
}


#if UNITY_EDITOR
[ExecuteInEditMode]
[CustomEditor(typeof(FishingBox)), CanEditMultipleObjects]
public class FishingBoxEditor : Editor
{
    private void OnSceneGUI()
    {
        EditorGUI.BeginChangeCheck();
        FishingBox fb = (FishingBox)target;
        fb.position = Handles.PositionHandle(fb.position, Quaternion.identity);

        if(EditorGUI.EndChangeCheck())
        {
            EditorUtility.SetDirty(fb);
        }
    }
}
#endif