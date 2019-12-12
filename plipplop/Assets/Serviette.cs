using UnityEngine;
public class Serviette : Activity
{
	[Header("SERVIETTE")]
	public float offset;

	public override void Enter(NonPlayableCharacter user)
	{
		base.Enter(user);
		user.transform.position = transform.position + transform.forward * offset;
		user.transform.forward = -transform.forward;
		user.agentMovement.Stop();
		user.agentMovement.ClearEvents();
		user.animator.SetBool("Tanning", true);
		full = true;
	}

	public override void Exit(NonPlayableCharacter user)
	{
		user.animator.SetBool("Tanning", false);
		full = false;
		base.Exit(user);
	}

#if UNITY_EDITOR
	void OnDrawGizmosSelected()
	{
		Gizmos.color = new Color32(255, 215, 0, 255);
		UnityEditor.Handles.DrawWireCube(transform.position + transform.forward * offset, Vector3.one * 0.25f);
	}
#endif
}
