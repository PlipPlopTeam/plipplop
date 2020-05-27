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

	public override void StartUsing(NonPlayableCharacter user)
	{
		base.StartUsing(user);
		full = true;
		user.movement.SetSpeed(speed);
		user.movement.FollowPath(path);
		transform.SetParent(user.transform);
		transform.localPosition = Vector3.zero;
		transform.forward = user.transform.forward;
		collider.enabled = false;
		rb.isKinematic = true;
	}
	public override void StopUsing(NonPlayableCharacter user)
	{
		base.StopSpectate(user);
		user.movement.ResetSpeed();
		full = false;
		transform.SetParent(null);
		collider.enabled = true;
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
        
        AIPath.Point[] newPath = b.path.points.ToArray();
        if(newPath.Length == 0) return;
        for(int i = 0; i < newPath.Length; i++)
        {
            newPath[i].position = Handles.PositionHandle(newPath[i].position, Quaternion.identity);
        }

        if(EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(b, "Move Bike path point");
            b.path.points = new List<AIPath.Point>(newPath);
            EditorUtility.SetDirty(b);
        }
    }
}
#endif