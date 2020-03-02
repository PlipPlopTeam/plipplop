using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Radio : Hopper
{
    [RequireComponent(typeof(Jukebox))]
    [System.Serializable]
    public class Music
    {
        public string name;
        public int bpm;
    }

    [Header("RADIO SETTINGS")]
    public List<Music> musics = new List<Music>();

    bool isOn = false;
    int currentMusic = -1;
    Jukebox jukebox;
    float timeSinceLastBeat = 0f;
    float noiseBase = 0.2f;
    float noiseReduxFactor = 15f;
    float pitch = 1f;
    AudioSource noiseSrc;

    internal override void Awake()
    {
        base.Awake();
        jukebox = GetComponent<Jukebox>();
        jukebox.doesPlayMusicForReal = false;
        jukebox.animateWithSound = false;
    }

    internal override void Shout()
    {
        SoundPlayer.PlaySoundAttached("sfx_radio_click", this.transform);
        if (isOn) {
            StopMusic();
            SoundPlayer.StopSound(noiseSrc);
        }
        else {
            PlayMusic();
            noiseSrc = SoundPlayer.PlaySoundAttached("bgs_radio_static", this.transform, noiseBase);
        }
    }

    internal override void Update()
    {
        base.Update();
        if (isOn) {
            timeSinceLastBeat += Time.deltaTime;
            var delta = jukebox.GetAnimDelta();
            if (timeSinceLastBeat > 1f/((musics[currentMusic].bpm* pitch) / 60f)) {
                timeSinceLastBeat = 0f;
                jukebox.SetAnimDelta(1f);
            }
            else if (delta > 0f) {
                jukebox.SetAnimDelta(delta - Time.deltaTime);
            }

            noiseSrc.volume = Mathf.Clamp01(rigidbody.velocity.magnitude * (1f - noiseBase) * (1f/ noiseReduxFactor)) + noiseBase;
            jukebox.GetAttachedSource().volume = 1f - noiseSrc.volume;
            pitch = 0.95f + noiseSrc.volume * 0.1f;
            jukebox.GetAttachedSource().pitch = pitch;
        }
        else {
            jukebox.SetAnimDelta(0f);
        }
    }

    void PlayMusic()
    {
        if (musics.Count == 0) {
            Debug.LogError("Tried to play music with radio " + name + " but the music list is EMPTY!");
            return;
        }
        isOn = true;
        currentMusic = (currentMusic + 1) % musics.Count;
        jukebox.music = musics[currentMusic].name;
        jukebox.PlayMusic();
        jukebox.GetAttachedSource().volume = 1f - noiseBase;
        timeSinceLastBeat = 0f;
    }

    void StopMusic()
    {
        isOn = false;
        jukebox.StopMusic();
    }

    public bool IsRadioOn()
    {
        return isOn;
    }
}
