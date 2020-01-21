using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[RequireComponent(typeof(PlayableDirector))]
public class SpielbergAssistant : MonoBehaviour
{
    public enum EStartType { SMOOTH_MOVEMENT, CUT }; // TODO: Add FADE...

    public bool paralyzeOnStart = true;
    public bool liberateOnEnd = true;
    public EStartType startType = EStartType.SMOOTH_MOVEMENT;
    public EStartType endType = EStartType.SMOOTH_MOVEMENT;
    [Range(0.3f, 10f)] public float smoothMovementSpeed = 1f;

    public Camera firstCamera;

    bool isPlaying = false;
    Camera lastCamera;
    List<Camera> cameraHistory = new List<Camera>();

    PlayableDirector director;

    private void Awake()
    {
        director = GetComponent<PlayableDirector>();
        director.stopped += OnCinematicEnded;
    }

    void OnCinematicEnded(PlayableDirector director)
    {
        isPlaying = false;

        if (firstCamera) StartCoroutine(PrepareCinematicEnding());
        else EndInternal();
        if (liberateOnEnd) LiberatePlayer();
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
        if (isPlaying) {
            return;
        }

        foreach(var cam in GetComponentsInChildren<Camera>()) {
            cam.gameObject.SetActive(false);
        }

        if (paralyzeOnStart) ParalyzePlayer();

        isPlaying = true;
        lastCamera = Aperture.GetCurrentlyActiveCamera();
        cameraHistory.Add(lastCamera);

        if (firstCamera) StartCoroutine(PrepareCinematic());
        else PlayInternal();
    }

    private void Update()
    {
        if (isPlaying && firstCamera) {
            var currCamera = Aperture.GetCurrentlyActiveCamera();
            if (!Object.ReferenceEquals(currCamera, lastCamera)) {
                cameraHistory.Add(lastCamera);
                lastCamera = currCamera;
            }
        }   
    }

    IEnumerator PrepareCinematic()
    {
        Debug.Log("Spielberg assistant is PREPARING a cinematic");
        var aperture = Game.i.aperture;
        var initialPos = new Geometry.PositionAndRotation() { position = firstCamera.transform.position, rotation = firstCamera.transform.rotation };
        var objective = aperture.AddStaticObjective(new Aperture.StaticObjective(initialPos) { manualLerp = 0f });
        aperture.Load(AperturePreset.CreateFromCamera(firstCamera));
        aperture.FixedUpdate();

        if (startType == EStartType.CUT) {
            aperture.Teleport();
            PlayInternal();
        }

        else if (startType == EStartType.SMOOTH_MOVEMENT) {
            aperture.DisableLookAt();
            while (aperture.IsTransitioningOnStack() || aperture.IsMovingToDestination()) {
                // Aperture will position itself each frame
                objective.manualLerp += Time.deltaTime * (smoothMovementSpeed);
                yield return new WaitForFixedUpdate();
            }
            PlayInternal();
            aperture.EnableLookAt();
        }
        aperture.RemoveStaticPosition(initialPos);
    }

    IEnumerator PrepareCinematicEnding()
    {
        var currCam = Aperture.GetCurrentlyActiveCamera();
        var aperture = Game.i.aperture;
        var endPos = new Geometry.PositionAndRotation() { position = currCam.transform.position, rotation = currCam.transform.rotation };
        var objective = aperture.AddStaticObjective(new Aperture.StaticObjective(endPos) { manualLerp = 0f });
        Game.i.aperture.Load(AperturePreset.CreateFromCamera(currCam));
        Game.i.aperture.SwapLastTwoStackElements();
        aperture.FixedUpdate();

        if (endType == EStartType.CUT) {
            aperture.Teleport();
            EndInternal();
        }

        else if (endType == EStartType.SMOOTH_MOVEMENT) {
            while (aperture.IsTransitioningOnStack() || aperture.IsMovingToDestination()) {
                objective.manualLerp += Time.deltaTime * smoothMovementSpeed;
                yield return new WaitForFixedUpdate();
            }
            EndInternal();
        }

        aperture.RemoveStaticPosition(endPos);
    }

    void PlayInternal()
    {
        if (firstCamera) {
            Game.i.aperture.SwitchCamera(firstCamera);
        }
        Game.i.aperture.UnloadLast();
        Game.i.aperture.Freeze();
        director.Play();
    }

    void EndInternal()
    {
        if (firstCamera) {
            Debug.Log("Ending cinematic and giving back control to : "+cameraHistory[0]);
            Game.i.aperture.SwitchCamera(cameraHistory[0]);
        }
        Game.i.aperture.Unfreeze();
    }
}
