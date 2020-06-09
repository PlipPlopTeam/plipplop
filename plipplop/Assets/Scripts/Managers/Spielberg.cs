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

    public static void PlayCinematic(GameObject cinematicObj)
    {
        Game.i.cinematics.Play(cinematicObj);
    }

    public void Play(string cinematicName)
    {
        if (currentAssistant != null) return;
        var kino = Game.i.library.cinematics.Find(o => o.name == cinematicName);
        if (kino == null) {
            Debug.LogError("SPIELBERG ERROR: Cinematic not found (" + cinematicName + "). Please check the library.");
            return;
        }

        var cineObj = GameObject.Instantiate(kino.prefab);
        Play(cineObj);
    }

    public void Play(GameObject cinematicObj)
    {
        if (currentAssistant != null) return;
        currentAssistant = cinematicObj.GetComponent<SpielbergAssistant>();
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

    public void KinoStartDialogue(string dialogName)
    {
        // On mwon mwon only for now
        var talking = GameObject.FindObjectOfType<MwonMwonIntroQuest>();
        Game.i.PlayDialogue(Game.i.library.dialogs[dialogName], talking);
    }

    public void KinoDialogueNextStep()
    {

    }

    public void KinoWaitEndOfDialogue()
    {
        currentAssistant.PauseAndWaitForEndOfDialogue();
    }

    public void KinoWaitForInput(EAction action)
    {
        currentAssistant.PauseAndWaitForInput(action);
    }

    public void KinoDisableNPC(string npcName)
    {
        currentAssistant.ToggleNPCAI(npcName, false);
    }

    public void KinoEnableNPC(string npcName)
    {
        currentAssistant.ToggleNPCAI(npcName, false);
    }

    public void KinoNPCGoTo(string npcGoTo, string objectiveName)
    {
        currentAssistant.NPCGoTo(npcGoTo, objectiveName);
    }

    public void KinoNPCPanic(string npcGoTo)
    {
        currentAssistant.NPCPanic(npcGoTo);
    }

    public void KinoNPCUseActivity(string npc, string act)
    {
        currentAssistant.ToggleNPCActivity(npc, act, true);
    }

    public void KinoNPCStopUsingActivity(string npc, string act)
    {
        currentAssistant.ToggleNPCActivity(npc, act, false);
    }

    public void KinoNPCEject(NonPlayableCharacter npc, Emotion.EVerb verb, string[] subjects, Emotion.EBubble bubbleType)
    {
        npc.emo.Show(verb, subjects, bubbleType);
    }   

    public void KinoScreenShake(float time, float force)
    {
        Game.i.aperture.Shake(force, time);
    }
}
