using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jukebox : Activity
{
	[Header("Jukebox")]
	public Vector2 radius;

    public ParticleSystem ps;
	Transform visuals;

	private void Start()
    {
        visuals = transform.GetChild(0);
    }

    private void OnEnable()
    {
        SoundPlayer.PlaySoundAttached("bgm_test", transform);
        ps.Play();
    }

    private void OnDisable()
    {
        SoundPlayer.StopSound("bgm_test");
        ps.Stop();
    }

    public override void Update() 
    {
        base.Update();
        visuals.localScale = Vector3.one + Vector3.one * (1 + Mathf.Sin(Time.time * 10f) )* 0.1f;
    }

	public override void StartUsing(NonPlayableCharacter user)
	{
		base.StartUsing(user);

		Vector3 pos = Geometry.GetRandomPointAround(Random.Range(radius.x, radius.y)) + transform.position;
		user.agentMovement.GoThere(pos, true);
		user.agentMovement.onDestinationReached += () =>
		{
			user.transform.LookAt(transform.position);
			user.animator.SetBool("Dancing", true);
		};
	}
	public override void StopUsing(NonPlayableCharacter user)
	{
		base.StopUsing(user);
		user.animator.SetBool("Dancing", false);
	}

#if UNITY_EDITOR
	public override void OnDrawGizmosSelected()
	{
		base.OnDrawGizmosSelected();
		UnityEditor.Handles.color = new Color32(255, 215, 0, 255);
		UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.up, radius.x);
		UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.up, radius.y);
	}

	void OnValidate()
	{
		if (radius.x < 0) radius.x = 0;
		if (radius.y < radius.x) radius.y = radius.x;
	}
#endif
}
