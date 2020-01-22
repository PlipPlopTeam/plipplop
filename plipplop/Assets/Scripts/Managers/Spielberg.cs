using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spielberg
{
    public enum EAction
    {
        SWITCH_CAMERA,
        PLAY_GAMEFX,
        PLAY_SOUND
    }

    Dictionary<EAction, System.Action<string[]>> bindings = new Dictionary<EAction, System.Action<string[]>>();
    SpielbergAssistant currentAssistant = null;

    public Spielberg()
    {
        bindings[EAction.SWITCH_CAMERA] = SwitchCamera;
        bindings[EAction.PLAY_GAMEFX] = PlaySound;
        bindings[EAction.PLAY_SOUND] = PlayGameEffect;
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

    public void Bind(EAction action, string[] args)
    {
        try {
            bindings[action](args);
        }
        catch (System.IndexOutOfRangeException) {
            Debug.LogError("You provided the WRONG NUMBER OF ARGUMENTS for Spielberg action " + action + " (provided: " + args.Length + ", expected more)");
        }
    }

    // BINDINGS
    void SwitchCamera(string[] args)
    {
        currentAssistant.SwitchCamera(args[0]);   
    }

    void PlaySound(string[] args)
    {
        SoundPlayer.Play(args[0]);
    }

    void PlayGameEffect(string[] args)
    {

    }

}
