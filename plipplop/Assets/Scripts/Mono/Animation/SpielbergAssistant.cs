using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[RequireComponent(typeof(PlayableDirector))]
public class SpielbergAssistant : MonoBehaviour
{
    public enum ETransitionType { SMOOTH_MOVEMENT, CUT }; // TODO: Add FADE...

    public bool paralyzeOnStart = true;
    public bool liberateOnEnd = true;
    public ETransitionType startType = ETransitionType.SMOOTH_MOVEMENT;
    public ETransitionType endType = ETransitionType.SMOOTH_MOVEMENT;
    [Range(0.3f, 10f)] public float smoothMovementSpeed = 1f;

    public Camera firstCamera;

    bool isPlaying = false;
    Camera lastCamera;

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
            cam.enabled = false;
        }

        if (paralyzeOnStart) ParalyzePlayer();

        isPlaying = true;
        lastCamera = Aperture.GetCurrentlyActiveCamera();

        if (firstCamera) StartCoroutine(PrepareCinematic());
        else PlayInternal();
    }

    private void Update()
    {
        if (isPlaying && firstCamera) {
            var currCamera = Aperture.GetCurrentlyActiveCamera();
            if (!Object.ReferenceEquals(currCamera, lastCamera)) {
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

        if (startType == ETransitionType.CUT) {
            aperture.Teleport();
            PlayInternal();
        }

        else if (startType == ETransitionType.SMOOTH_MOVEMENT) {
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

        if (endType == ETransitionType.CUT) {
            aperture.Teleport();
            EndInternal();
        }

        else if (endType == ETransitionType.SMOOTH_MOVEMENT) {
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
            SwitchCamera(firstCamera.transform);
        }
        Game.i.aperture.UnloadLast();
        Game.i.aperture.Freeze();
        director.Play();
    }

    void EndInternal()
    {
        Game.i.aperture.currentCamera.transform.parent = null;
        Game.i.aperture.Unfreeze();
    }

    public void SwitchCamera(string cameraName)
    {
        var t = transform.Find(cameraName);
        if (t == null) {
            Debug.LogError("SPIELBERG ERROR: Camera " + cameraName + " does not exist or is not a CHILD of SpielbergAssistant");
        }
        SwitchCamera(t);
    }

    public void SwitchCamera(Transform newCamera)
    {
        Game.i.aperture.currentCamera.transform.parent = newCamera.transform;
        if (newCamera != null) {
            Game.i.aperture.currentCamera.transform.localPosition = Vector3.zero;
            Game.i.aperture.currentCamera.transform.localRotation = Quaternion.identity;
        }
    }
}
