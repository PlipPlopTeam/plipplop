using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundPlayer
{
    static AudioSource source;

    public class MissingSoundException : System.Exception { public MissingSoundException(string msg) { Debug.LogError("COULD NOT FIND SOUND NAMED [" + msg + "]\nDid you type the name correctly?"); } };
    
    // General function
    static void PlayClipOnce(AudioClip clip, AudioSource source, float volume = 1f, float pitch = 1f)
    {
        source.pitch = pitch;
        source.PlayOneShot(clip, volume);
    }

    static void LoopClip(AudioClip clip, float volume = 1f, float pitch = 1f, AudioSource src=null)
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
    }

    static void PlaySound(Sound snd, float volume=1f, float pitch = 1f, AudioSource src=null)
    {
        if (source == null) { source = Camera.main.gameObject.AddComponent<AudioSource>(); }
        if (src == null) { src = source; }

        if (snd.loop) {
            MakeUnique(snd);
            LoopClip(snd.clip, volume, pitch);
        }
        else {
            PlayClipOnce(snd.clip, src, volume, pitch);
        }
    }

    // Public Specifics
    public static void Play(string soundName, float volume=1f, float pitch=1f)
    {
        var snd = GetSoundFromName(soundName);
        PlaySound(snd, volume, pitch);
    }

    public static void PlayWithRandomPitch(string soundName, float volume = 1f)
    {
        PlaySound(GetSoundFromName(soundName), volume, RandomPitch()); 
    }

    public static void PlaySoundAttached(string soundName, Transform parent, float volume = 1f, bool randomPitch = false)
    {
        var snd = GetSoundFromName(soundName);
        var clip = snd.clip;

        var g = new GameObject();
        g.name = "_ATTACHED_PLAYER";
        var source = g.AddComponent<AudioSource>();
        source.spatialBlend = 1f;
        source.minDistance = 2000f;
        source.maxDistance = 2000.1f;
        if (snd.loop == false) {
            g.AddComponent<DestroyAfter>().lifespan = clip.length + 1f;
        }
        g.transform.parent = parent;
        g.transform.localPosition = new Vector3();

        PlaySound(snd, volume, randomPitch ? RandomPitch() : 1f, source);
    }

    public static void PlayAtPosition(string soundName, Vector3 position, float volume = 1f, bool randomPitch = false)
    {
        var g = new GameObject();
        g.name = "_SPATIALIZED_PLAYER";
        g.transform.position = position;

        var snd = GetSoundFromName(soundName);
        var clip = snd.clip;
        if (snd.loop == false) {
            g.AddComponent<DestroyAfter>().lifespan = clip.length + 1f;
        }

        PlaySoundAttached(soundName, g.transform, volume, randomPitch);
    }

    // utilities

    public static void StopEverySound()
    {
        foreach (var src in GameObject.FindObjectsOfType<AudioSource>()) {
            src.Stop();
            if (src == source) continue;
            GameObject.Destroy(src.gameObject);
        }
    }

    public static void StopSound(string soundName)
    {
        var snd = GetSoundFromName(soundName);
        foreach (var src in GameObject.FindObjectsOfType<AudioSource>()) {
            if (src.clip != snd.clip) continue;
            src.Stop();
            if (src == source) continue;
            GameObject.Destroy(src.gameObject);
        }
    }

    static float RandomPitch()
    {
        return 1 - 0.2f + Random.value / 2.5f; // +/- 0.2
    }

    static void MakeUnique(Sound sound)
    {
        foreach(var src in GameObject.FindObjectsOfType<AudioSource>()) {
            if (src.clip == sound.clip) {
                src.Stop();
                if (src == source) continue;
                GameObject.Destroy(src.gameObject);
            }
        }
    }

    static Sound GetSoundFromName(string name)
    {
        foreach(Sound s in Game.i.library.sounds)
        {
            if(s.name == name) return s;
        }
        throw new MissingSoundException(name);
    }
}
