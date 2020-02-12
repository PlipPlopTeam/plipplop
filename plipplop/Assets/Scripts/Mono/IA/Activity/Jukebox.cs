using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jukebox : Activity
{
	[Header("Jukebox")]
	public Vector2 radius;
    public ParticleSystem ps;
    public bool doesPlayMusicForReal = true;
    public string music = "bgm_test";
    public bool animateWithSound = true;

    float animDelta = 0f;
    float radioBeatScale = 0.4f;
    Transform visuals;
    AudioSource attachedSource;

    public void SetAnimDelta(float v)
    {
        animDelta = v;
    }

    public float GetAnimDelta()
    {
        return animDelta;
    }

    private void Start()
    {
        visuals = transform.GetChild(0);
    }

    public void PlayMusic()
    {
        attachedSource = SoundPlayer.PlaySoundAttached(music, transform);
        ps.Play();
    }

    public AudioSource GetAttachedSource()
    {
        return attachedSource;
    }

    public void StopMusic()
    {
        if (attachedSource != null) {
            SoundPlayer.StopSound(attachedSource);
        }
        ps.Stop();
    }

    private void OnEnable()
    {
        if (doesPlayMusicForReal) {
            PlayMusic();
        }
    }

    private void OnDisable()
    {
        if (doesPlayMusicForReal) {
            StopMusic();
        }
    }

    public override void Update() 
    {
        base.Update();
        if (animateWithSound) {
            animDelta = (1 + Mathf.Sin(Time.time * 10f)) * 0.1f;
        }
        visuals.localScale = Vector3.one + Vector3.one * animDelta * radioBeatScale;
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
