using System.Collections;
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
			user.Carry(Instantiate(fishingPole).GetComponent<FishingPole>());
            Vector3 pos = position + Geometry.GetRandomPointOnCircle(radius);
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

			FishingPole fp = user.carried.Self().GetComponent<FishingPole>();
			fp.Use();
			fp.Plunge(position + Geometry.GetRandomPointInRange(radius));

			user.look.FocusOn(fp.plug);
			user.agentMovement.OrientToward(fp.plug.position);
		};
    }

	public override void StopUsing(NonPlayableCharacter user)
	{
		base.StopUsing(user);
		user.skeleton.Drop(Cloth.ESlot.RIGHT_HAND);
		user.animator.SetBool("Fishing", false);
		StopAllCoroutines();
	}


#if UNITY_EDITOR
	void OnDrawGizmosSelected()
    {
		GUIStyle s = new GUIStyle();
		s.alignment = TextAnchor.MiddleCenter;
		s.fontStyle = FontStyle.Bold;
		s.normal.textColor = Color.white;
		Vector3 d = (transform.position - position).normalized;
		Vector3 p = position + (d * radius);

		Handles.color = new Color32(255, 255, 255, 255);
		UnityEditor.Handles.Label(position, "Fishing Area", s);
		UnityEditor.Handles.DrawLine(transform.position, p);
		UnityEditor.Handles.DrawWireDisc(position, Vector3.up, radius);

		Handles.color = new Color32(0, 0, 255, 50);
		UnityEditor.Handles.DrawSolidArc(position, Vector3.up, Vector3.right, 360f, radius);

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