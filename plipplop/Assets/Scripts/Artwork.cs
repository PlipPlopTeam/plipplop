using UnityEngine;

public class Artwork : Activity
{
	[Header("ARTWORK")]
	public Vector3 focusOffset;
	public float radius = 2f;

	public override void Enter(NonPlayableCharacter user)
	{
		base.Enter(user);

		Vector3 pos = Geometry.GetRandomPointAround(radius) + transform.position;
		user.agentMovement.Stop();
		user.agentMovement.GoThere(pos);
		user.agentMovement.ClearEvents();
		user.agentMovement.onDestinationReached += () =>
		{
			user.transform.LookAt(transform.position);
			user.look.FocusOn(transform.position + focusOffset);
		};
	}

	public override void Exit(NonPlayableCharacter user)
	{
		user.look.LooseFocus();
		base.Exit(user);
	}

#if UNITY_EDITOR
	void OnDrawGizmosSelected()
	{
		Gizmos.color = new Color32(255, 215, 0, 255);
		UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.up, radius);
		UnityEditor.Handles.DrawWireCube(transform.position + focusOffset, Vector3.one * 0.25f);
	}
#endif
}
