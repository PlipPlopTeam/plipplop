using UnityEngine;
public class Serviette : Activity
{
	[Header("SERVIETTE")]
	public float offset;

	public override void StartUsing(NonPlayableCharacter user)
	{
		base.StartUsing(user);
		user.movement.GoThere(transform.position, true);
		user.movement.onDestinationReached += () =>
		{
			user.transform.position = transform.position + transform.forward * offset;
			user.movement.Orient(-transform.forward);
			user.animator.SetBool("Tanning", true);
			user.movement.Stop();
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
