using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spielberg
{
    public enum EAction
    {
        SWITCH_CAMERA,
        PLAY_GAMEFX,
        PLAY_SOUND,
        PLAY_MUSIC,
        STOP_MUSIC,
        PARALYZE_PLAYER,
        LIBERATE_PLAYER,
        START_DIALOGUE,
        NEXT_DIALOGUE,
        WAIT_UNTIL_NEXT_DIALOGUE
    }

    Dictionary<EAction, Action<string[]>> bindings = new Dictionary<EAction, System.Action<string[]>>();
    SpielbergAssistant currentAssistant = null;

    public Spielberg()
    {
        bindings[EAction.SWITCH_CAMERA] = SwitchCamera;
        bindings[EAction.PLAY_GAMEFX] = PlaySound;
        bindings[EAction.PLAY_SOUND] = PlayGameEffect;
        bindings[EAction.PLAY_MUSIC] = PlayMusic;
        bindings[EAction.STOP_MUSIC] = StopMusic;
        bindings[EAction.PARALYZE_PLAYER] = ParalyzePlayer;
        bindings[EAction.LIBERATE_PLAYER] = LiberatePlayer;
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
        catch (KeyNotFoundException) {
            Debug.LogError("The Spielberg feature "+action+" is not bound to anything. Is that feature ready yet? Ask your programming team.");
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
        Pyromancer.PlayGameEffect(args[0], new Vector3(Convert.ToSingle(args[1]), Convert.ToSingle(args[2]), Convert.ToSingle(args[3])));
    }

    void PlayMusic(string[] args)
    {
        var musicName = args[0];
        var shouldFade = Convert.ToBoolean(args[1]);
        var volume = Convert.ToSingle(args[2]);

        SoundPlayer.Play(musicName, volume, 1f, shouldFade);
    }

    void StopMusic(string[] args)
    {
        var musicName = args[0];
        var shouldFade = Convert.ToBoolean(args[1]);
        SoundPlayer.StopSound(musicName, shouldFade);
    }

    void ParalyzePlayer(string[] args)
    {
        currentAssistant.ParalyzePlayer();
    }

    void LiberatePlayer(string[] args)
    {
        currentAssistant.LiberatePlayer();
    }
}
