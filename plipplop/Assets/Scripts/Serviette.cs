using UnityEngine;
public class Serviette : Activity
{
	[Header("SERVIETTE")]
	public float offset;

	public override void StartUsing(NonPlayableCharacter user)
	{
		base.StartUsing(user);

		user.agentMovement.GoThere(transform.position, true);
		user.agentMovement.onDestinationReached += () =>
		{
			user.transform.position = transform.position + transform.forward * offset;
			user.transform.forward = -transform.forward;
			user.animator.SetBool("Tanning", true);
			user.agentMovement.Stop();
		};
	}
	public override void StopUsing(NonPlayableCharacter user)
	{
		base.StopUsing(user);
		if (user.animator != null) user.animator.SetBool("Tanning", false);
	}

#if UNITY_EDITOR
	void OnDrawGizmosSelected()
	{
		Gizmos.color = new Color32(255, 215, 0, 255);
		UnityEditor.Handles.DrawWireCube(transform.position + transform.forward * offset, Vector3.one * 0.25f);
	}
#endif
}
