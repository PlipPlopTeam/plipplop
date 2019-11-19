using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jukebox : Activity
{
    Transform visuals;
    public ParticleSystem ps;

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
        visuals.localScale = Vector3.one + Vector3.one * (1 + Mathf.Sin(Time.time * 10f)) * 0.25f;
    }

    public override void Enter(NonPlayableCharacter user)
    {
        base.Enter(user);
        user.agentMovement.Stop();
        user.animator.SetBool("Dancing", true);
    }

    public override void Exit(NonPlayableCharacter user)
    {
        base.Exit(user);
        user.animator.SetBool("Dancing", false);
    }
}
