using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jukebox : Activity
{
    Transform visuals;

    private void Start()
    {
        visuals = transform.GetChild(0);
    }

    private void OnEnable()
    {
        SoundPlayer.PlaySoundAttached("bgm_test", transform);
    }

    private void OnDisable()
    {
        SoundPlayer.StopSound("bgm_test");
    }

    public override void Update() {
        visuals.localScale = Vector3.one + Vector3.one * (1 + Mathf.Sin(Time.time * 10f)) * 0.25f;

        foreach (NonPlayableCharacter user in users) {
            user.transform.Rotate(user.transform.up * Time.deltaTime * 250f);
        }
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
