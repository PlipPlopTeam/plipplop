using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class SoundPlayer
{
    static AudioSource source;

    static readonly float fadeSpeed = 1f;
    static List<AudioSource> managedSources = new List<AudioSource>();

    public class MissingSoundException : System.Exception { public MissingSoundException(string msg) { Debug.LogError("COULD NOT FIND SOUND NAMED [" + msg + "]\nDid you type the name correctly?"); } };
    
    // General function
    static void PlayClipOnce(AudioClip clip, AudioSource source, float volume = 1f, float pitch = 1f)
    {
        source.pitch = pitch;
        source.PlayOneShot(clip, volume);
    }

    static AudioSource LoopClip(AudioClip clip, float volume = 1f, float pitch = 1f, AudioSource src=null)
    {
        if (src == null) {
            var g = new GameObject();
            g.name = "_SOUND_LOOPER";
            src = g.AddComponent<AudioSource>();
        }
        src.loop = true;
        src.pitch = pitch;
        src.volume = volume;
        src.clip = clip;
        src.Play();

        return src;
    }

    static AudioSource PlaySound(Sound snd, float volume=1f, float pitch = 1f, AudioSource src=null, bool shouldFadeIn = false)
    {
        try
        {
            if (source == null)
            {
                source = Camera.main.gameObject.AddComponent<AudioSource>();
            }

            if (src == null)
            {
                src = source;
            }

            if (snd.loop)
            {
                MakeUnique(snd);
                src = LoopClip(snd.clip, shouldFadeIn ? 0f : volume, pitch);
                if (shouldFadeIn) {
                    UnityMainThreadDispatcher.Instance().StartCoroutine(FadeVolumeOverTime(src, volume));
                }
                if (src != source) managedSources.Add(src);
                return src;
            }
            else
            {
                PlayClipOnce(snd.clip, src, volume, pitch);
                if (src != source) managedSources.Add(src);
                return src;
            }
        }
        catch (NullReferenceException)
        {
            Debug.LogWarning("Could not play the sound "+snd.name+" because either the camera or source does NOT exist yet.");
            return null;
        }
    }

    // Public Specifics
    public static void Play(string soundName, float volume=1f, float pitch=1f, bool shouldFadeIn=false)
    {
        var snd = GetSoundFromName(soundName);
        PlaySound(snd, volume, pitch, null, shouldFadeIn);
    }

    public static void PlayWithRandomPitch(string soundName, float volume = 1f)
    {
        PlaySound(GetSoundFromName(soundName), volume, RandomPitch()); 
    }

    public static AudioSource PlaySoundAttached(string soundName, Transform parent, float volume = 1f, bool randomPitch = false, bool shouldFadeIn = false)
    {
        var snd = GetSoundFromName(soundName);
        var clip = snd.clip;

        var g = new GameObject();
        g.name = "_ATTACHED_PLAYER";
        var source = g.AddComponent<AudioSource>();
        source.spatialBlend = 1f;
        source.minDistance = 5f;
        source.maxDistance = 35f;
        if (snd.loop == false) {
            g.AddComponent<DestroyAfter>().lifespan = clip.length + 1f;
        }
        g.transform.parent = parent;
        g.transform.localPosition = new Vector3();

        return PlaySound(snd, volume, randomPitch ? RandomPitch() : 1f, source, shouldFadeIn);
    }

    public static void PlayAtPosition(string soundName, Vector3 position, float volume = 1f, bool randomPitch = false, bool shouldFadeIn = false)
    {
        var g = new GameObject();
        g.name = "_SPATIALIZED_PLAYER";
        g.transform.position = position;

        var snd = GetSoundFromName(soundName);
        var clip = snd.clip;
        if (snd.loop == false) {
            g.AddComponent<DestroyAfter>().lifespan = clip.length + 1f;
        }

        PlaySoundAttached(soundName, g.transform, volume, randomPitch, shouldFadeIn);
    }

    // utilities

    public static void StopEverySound()
    {
        CleanSources();
        foreach (var src in managedSources) {
            src.Stop();
            if (src == source) continue;
            managedSources.Remove(src);
            GameObject.Destroy(src.gameObject);
        }
    }

    public static void StopSound(string soundName, bool shouldFade = false) { StopSounds(soundName, shouldFade); }
    public static void StopSounds(string soundName, bool shouldFade = false)
    {
        var snd = GetSoundFromName(soundName);
        foreach (var src in managedSources) {
            if (src.clip != snd.clip) continue;
            StopSound(src, shouldFade);
        }
    }

    public static void StopSound(AudioSource src, bool shouldFade = false)
    {
        Action cleanSteps = delegate {
            src.Stop();
            if (src == source) return;
            managedSources.Remove(src);
            GameObject.Destroy(src.gameObject);
        };
        if (shouldFade) {
            UnityMainThreadDispatcher.Instance().StartCoroutine(FadeVolumeOverTime(src, 0f, cleanSteps));
        }
        else {
            cleanSteps.Invoke();
        }
    }

    static float RandomPitch()
    {
        return 1 - 0.2f + Random.value / 2.5f; // +/- 0.2
    }

    static void MakeUnique(Sound sound)
    {
        CleanSources();
        foreach (var src in managedSources) {
            if (src.clip == sound.clip) {
                src.Stop();
                if (src == source) continue;
                GameObject.Destroy(src.gameObject);
            }
        }
    }

    static void CleanSources()
    {
        managedSources.RemoveAll(o=>o == null);
    }

    static Sound GetSoundFromName(string name)
    {
        foreach(Sound s in Game.i.library.sounds)
        {
            if(s.name == name) return s;
        }
        throw new MissingSoundException(name);
    }

    public static IEnumerator FadeVolumeOverTime(AudioSource src, float targetVolume, Action callback = null)
    {
        var originalVolume = src.volume;
        var state = 0f;
        while (state < 1f) {
            state += Time.deltaTime * fadeSpeed;
            src.volume = Mathf.Lerp(originalVolume, targetVolume, state);
            yield return new WaitForEndOfFrame();
        }
        if (callback != null)
            callback.Invoke();
    }
}
