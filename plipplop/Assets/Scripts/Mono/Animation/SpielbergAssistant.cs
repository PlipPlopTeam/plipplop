using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[RequireComponent(typeof(PlayableDirector))]
public class SpielbergAssistant : MonoBehaviour
{
    public enum EStartType { SMOOTH_MOVEMENT, CUT, FADE};

    public bool paralyzeOnStart = true;
    public bool liberateOnEnd = true;
    public EStartType startType = EStartType.SMOOTH_MOVEMENT;

    public Camera firstCamera;

    bool isPlaying = false;

    PlayableDirector director;

    private void Awake()
    {
        director = GetComponent<PlayableDirector>();
        director.stopped += OnCinematicEnded;
    }

    void OnCinematicEnded(PlayableDirector director)
    {

    }

    public void ParalyzePlayer() {
        Game.i.player.Paralyze();
    }

    public void LiberatePlayer()
    {
        Game.i.player.Deparalyze();
    }

    public void Play()
    {
        foreach(var cam in GetComponentsInChildren<Camera>()) {
            cam.gameObject.SetActive(false);
        }

        if (paralyzeOnStart) ParalyzePlayer();

        isPlaying = true;

        if (firstCamera) StartCoroutine(PrepareCinematic());
        else PlayInternal();
    }

    IEnumerator PrepareCinematic()
    {
        var aperture = Game.i.aperture;
        var initialPos = new Geometry.PositionAndRotation() { position = firstCamera.transform.position, rotation = firstCamera.transform.rotation };

        aperture.AddStaticPosition(initialPos);
        aperture.Load(AperturePreset.CreateFromCamera(firstCamera));

        if (startType == EStartType.CUT) {
            aperture.FixedUpdate();
            aperture.Teleport();
            PlayInternal();
            yield break;
        }

        if (startType == EStartType.SMOOTH_MOVEMENT) {
            while (aperture.IsTransitioningOnStack()) {
                yield return new WaitForEndOfFrame();
            }
            PlayInternal();
        }
    }

    void PlayInternal()
    {
        Game.i.aperture.cam.enabled = false;
        director.Play();
    }
}
