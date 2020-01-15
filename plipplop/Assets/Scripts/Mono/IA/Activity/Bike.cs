using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class Bike : Activity
{
    [Header("BIKE")]
    public float speed = 10f;
    public AIPath path;

    public Collider col;
    public Rigidbody rb;

    public override void Enter(NonPlayableCharacter user)
    {
        base.Enter(user);
        full = true;
        user.agentMovement.SetSpeed(speed);
        user.agentMovement.FollowPath(path);
        transform.SetParent(user.transform);
        transform.localPosition = Vector3.zero;
        transform.forward = user.transform.forward;
        col.enabled = false;
        rb.isKinematic = true;
    }

    public override void Exit(NonPlayableCharacter user)
    {
        base.Exit(user);
        // TODO : Give him back a path road for his patrol (dependent of the scene ?)
        user.agentMovement.ResetSpeed();
        full = false;
        transform.SetParent(null);
        col.enabled = true;
        rb.isKinematic = false;
    }
}

#if UNITY_EDITOR
[ExecuteInEditMode]
[CustomEditor(typeof(Bike)), CanEditMultipleObjects]
public class BikeEditor : Editor
{
    private void OnSceneGUI()
    {
        EditorGUI.BeginChangeCheck();

        Bike b = (Bike)target;
        if(b.path == null) return;
        
        Vector3[] newPath = b.path.points.ToArray();
        if(newPath.Length == 0) return;
        for(int i = 0; i < newPath.Length; i++)
        {
            newPath[i] = Handles.PositionHandle(newPath[i], Quaternion.identity);
        }

        if(EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(b, "Move Bike path point");
            b.path.points = new List<Vector3>(newPath);
            EditorUtility.SetDirty(b);
        }
    }
}
#endif