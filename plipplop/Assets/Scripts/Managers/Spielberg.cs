using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spielberg
{

    SpielbergAssistant currentAssistant = null;

    public Spielberg()
    {

    }

    public static void PlayCinematic(string cinematicName)
    {
        Game.i.cinematics.Play(cinematicName);
    }

    public void Play(string cinematicName)
    {
        if (currentAssistant != null) return;
        var kino = Game.i.library.cinematics.Find(o => o.name == cinematicName);
        if (kino == null) {
            Debug.LogError("SPIELBERG ERROR: Cinematic not found (" + cinematicName + "). Please check the library.");
            return;
        }
        currentAssistant = GameObject.Instantiate(kino.prefab).GetComponent<SpielbergAssistant>();
        currentAssistant.Play();
    }

    public void OnCinematicEnded()
    {
        GameObject.Destroy(currentAssistant.gameObject);
        currentAssistant = null;
    }

    public void KinoSwitchCamera(string cameraName)
    {
        currentAssistant.SwitchCamera(cameraName);   
    }

    public void KinoPlaySound(string sound, float volume, float pitch)
    {
        SoundPlayer.Play(sound, volume, pitch);
    }

    public void KinoPlayGameEffect(string gfx, Vector3 position)
    {
        Pyromancer.PlayGameEffect(gfx, currentAssistant.transform.position + position);
    }

    public void KinoPlayMusic(string musicName, float volume, bool shouldFade)
    {
        SoundPlayer.Play(musicName, volume, 1f, shouldFade);
    }

    public void KinoStopMusic(string musicName, bool shouldFade)
    {
        SoundPlayer.StopSound(musicName, shouldFade);
    }

    public void KinoParalyzePlayer()
    {
        currentAssistant.ParalyzePlayer();
    }

    public void KinoLiberatePlayer()
    {
        currentAssistant.LiberatePlayer();
    }

    public void KinoStartDialogue()
    {

    }

    public void KinoDialogueNextStep()
    {

    }

    public void PauseUntilDialogueAdvance()
    {

    }

    public void KinoScreenShake(float time, float force)
    {
        Game.i.aperture.Shake(force, time);
    }
}
