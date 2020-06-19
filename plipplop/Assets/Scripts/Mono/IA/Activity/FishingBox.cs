using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class FishingBox : Activity
{
    [System.Serializable]
    public class Area
    {
        public Vector3 position;
        public float radius;
    }

    [Header("FISHING-BOX")]
    public GameObject fishingPole;
    public Area plunge;
    public Area stand;


    public override void Enter(NonPlayableCharacter user)
    {
        base.Enter(user);
        user.movement.Stop();
        user.movement.GoThere(transform.position, true);
        user.movement.onDestinationReached += () =>
        {
			user.Carry(Instantiate(fishingPole).GetComponent<FishingPole>());
            Vector3 pos = stand.position + Geometry.GetRandomPointInRange(stand.radius);
			user.movement.Stop();
			user.movement.GoThere(pos, true);
			StartCoroutine(DelayedSetup(user));
        };
    }
    IEnumerator DelayedSetup(NonPlayableCharacter user)
    {
        yield return new WaitForEndOfFrame();
        user.movement.onDestinationReached += () =>
        {
			user.movement.Stop();
            user.animator.SetBool("Fishing", true);

			FishingPole fp = user.carried.Self().GetComponent<FishingPole>();
			fp.Use();
			fp.Plunge(plunge.position + Geometry.GetRandomPointInRange(plunge.radius));

			user.look.FocusOn(fp.plug);
			user.movement.OrientToward(fp.plug.position);
		};
    }

	public override void StopUsing(NonPlayableCharacter user)
	{
		base.StopUsing(user);
		user.Drop();
		user.animator.SetBool("Fishing", false);
		StopAllCoroutines();
	}


#if UNITY_EDITOR
	new void OnDrawGizmosSelected()
    {
        DrawArea(plunge.position, plunge.radius, Color.blue);
        DrawArea(stand.position, stand.radius, Color.green);
    }

    public void DrawArea(Vector3 position, float radius, Color color)
    {
    	GUIStyle s = new GUIStyle();
        s.alignment = TextAnchor.MiddleCenter;
		s.fontStyle = FontStyle.Bold;
		s.normal.textColor = color;

		Vector3 d = (transform.position - position).normalized;
        Vector3 p = position + (d * radius);

        Handles.color = new Color32(255, 255, 255, 255);
        //UnityEditor.Handles.Label(position, "Fishing Area", s);
		UnityEditor.Handles.DrawLine(transform.position, p);
		UnityEditor.Handles.DrawWireDisc(position, Vector3.up, radius);
        color.a = 0.25f;
		Handles.color = color;
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

        fb.plunge.position = Handles.PositionHandle(fb.plunge.position, Quaternion.identity);
        fb.stand.position = Handles.PositionHandle(fb.stand.position, Quaternion.identity);

        if (EditorGUI.EndChangeCheck())
        {
            EditorUtility.SetDirty(fb);
        }
    }
}
#endif