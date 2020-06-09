using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[RequireComponent(typeof(PlayableDirector))]
public class SpielbergAssistant : MonoBehaviour
{
    public enum ETransitionType { SMOOTH_MOVEMENT, CUT }; // TODO: Add FADE...
    public enum EParalyzisType { LIBERATE, PARALYZE, PARALYZE_AND_STOP, FREEZE, UNTOUCHED};

    public EParalyzisType playerStateOnStart = EParalyzisType.PARALYZE_AND_STOP;
    public EParalyzisType playerStateOnEnd = EParalyzisType.LIBERATE;
    public ETransitionType startType = ETransitionType.SMOOTH_MOVEMENT;
    public ETransitionType endType = ETransitionType.SMOOTH_MOVEMENT;
    [Range(0.3f, 10f)] public float smoothMovementSpeed = 1f;

    public Camera firstCamera;

    bool isPlaying = false;
    Camera lastCamera;

    PlayableDirector director;
    EAction? waitingOnInput;
    bool isWaitingOnDialogue;

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
        ExecuteParalyzisState(playerStateOnEnd);
    }

    public void ParalyzePlayer() {
        Game.i.player.Paralyze();
    }

    public void LiberatePlayer()
    {
        Game.i.player.Deparalyze();
    }

    [ContextMenu("TEST CINEMATIC")]
    public void PlayViaSpielberg_DO_NOT_USE_FROM_CODE_THIS_A_GUI_FUNCTION()
    {
        Spielberg.PlayCinematic(this.gameObject);
    }
    public void Play()
    {
        if (isPlaying) {
            return;
        }

        foreach(var cam in GetComponentsInChildren<Camera>()) {
            cam.enabled = false;
        }

        ExecuteParalyzisState(playerStateOnStart);

        isPlaying = true;
        lastCamera = Aperture.GetCurrentlyActiveCamera();

        if (firstCamera) StartCoroutine(PrepareCinematic());
        else PlayInternal();
    }

    private void Update()
    {
        if (isPlaying) {
            if (firstCamera) {
                var currCamera = Aperture.GetCurrentlyActiveCamera();
                if (!Object.ReferenceEquals(currCamera, lastCamera)) {
                    lastCamera = currCamera;
                }
            }


            if (waitingOnInput.HasValue && Game.i.mapping.IsPressed(waitingOnInput.Value)) {
                waitingOnInput = null;
                director.Resume();
            }

            if (isWaitingOnDialogue) {
                if (DialogHooks.currentInterlocutor == null) {
                    isWaitingOnDialogue = false;
                    director.Resume();
                }
            }
        }   
    }

    void ExecuteParalyzisState(EParalyzisType state)
    {
        switch (state) {
            case EParalyzisType.FREEZE:
                FreezePlayer();
                break;
            case EParalyzisType.LIBERATE:
                LiberatePlayer();
                UnfreezePlayer();
                break;
            case EParalyzisType.PARALYZE:
                ParalyzePlayer();
                break;
            case EParalyzisType.PARALYZE_AND_STOP:
                ParalyzeAndStopPlayer();
                break;
        }
    }

    public void FreezePlayer()
    {
        Game.i.player.FreezeController();
    }

    public void UnfreezePlayer()
    {
        Game.i.player.UnfreezeController();
    }

    public void ParalyzeAndStopPlayer()
    {
        ParalyzePlayer();
        Game.i.player.StopController();
    }

    IEnumerator PrepareCinematic()
    {
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

        aperture.RemoveStaticObjective(objective);
    }

    IEnumerator PrepareCinematicEnding()
    {
        var currCam = Aperture.GetCurrentlyActiveCamera();
        var aperture = Game.i.aperture;

        // Give current position to aperture
        aperture.SetCurrentPositionAndRotation(new Geometry.PositionAndRotation() { position = currCam.transform.position, rotation = currCam.transform.rotation });

        // Give final position to aperture
        var playerTransform = Game.i.player.GetCurrentController().transform;
        var endPos = new Geometry.PositionAndRotation() { position = playerTransform.TransformPoint(Vector3.back*aperture.GetSettings().distance.min + Vector3.up), rotation = playerTransform.rotation };
        var objective = aperture.AddStaticObjective(new Aperture.StaticObjective(endPos) { manualLerp = 0f });

        // Prepare aperture
        Game.i.aperture.Load(AperturePreset.CreateFromCamera(currCam));
        Game.i.aperture.SwapLastTwoStackElements();
        aperture.currentCamera.transform.parent = null;
        aperture.ComputeRotation();
        aperture.ComputePosition(Game.i.player.GetCurrentController().transform.position);

        if (endType == ETransitionType.CUT) {
            aperture.Teleport();
            EndInternal();
        }

        else if (endType == ETransitionType.SMOOTH_MOVEMENT) {
            Game.i.aperture.Unfreeze();
            Game.i.aperture.DisableLookAt();
            while (aperture.IsTransitioningOnStack() || aperture.IsMovingToDestination()) {
                objective.manualLerp += Time.deltaTime * smoothMovementSpeed;
                yield return new WaitForFixedUpdate();
            }
            Game.i.aperture.EnableLookAt();
            EndInternal();
        }

        aperture.RemoveStaticObjective(objective);
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
        Game.i.cinematics.OnCinematicEnded();
    }

    public void ToggleNPCAI(string npcName, bool toggle)
    {
        var npc = GetNPCByName(npcName);
        if (toggle) {
            npc.graph.Play();
        }
        else {
            npc.graph.Pause();
        }
    }

    public void ToggleNPCActivity(string npcName, string activity, bool toggle)
    {
        var npc = GetNPCByName(npcName);
        var t = GetChildInChildren(transform, activity);
        if (t == null) {
            Debug.LogError("SPIELBERG ERROR: Activity " + activity + " does not exist or is not a CHILD of SpielbergAssistant");
        }
        var activityComponent = t.GetComponent<Activity>();

        if (toggle) {
            activityComponent.Enter(npc);
        }
        else {
            activityComponent.Exit(npc);
        }
    }

    public void NPCPanic(string npcName)
    {
        var npc = GetNPCByName(npcName);

        npc.animator.SetBool("Scared", true);
        npc.emo.Show(Emotion.EVerb.SURPRISE, "plipplop");
    }

    public void NPCCalm(string npcName)
    {
        var npc = GetNPCByName(npcName);

        npc.animator.SetBool("Scared", false);
    }

    public void NPCGoTo(string npcName, string target)
    {
        var t = GetChildInChildren(transform, target);
        if (t == null) {
            Debug.LogError("SPIELBERG ERROR: TARGET " + target + " does not exist or is not a CHILD of SpielbergAssistant");
        }
        var targetPosition = t.position;
        var npc = GetNPCByName(npcName);

        npc.movement.GoThere(targetPosition, true);
    }

    NonPlayableCharacter GetNPCByName(string npcName)
    {
        var t = GetChildInChildren(transform, npcName);
        if (t == null) {
            Debug.LogError("SPIELBERG ERROR: NPC " + npcName + " does not exist or is not a CHILD of SpielbergAssistant");
        }
        var npc = t.GetComponent<NonPlayableCharacter>();
        return npc;
    }

    public void SwitchCamera(string cameraName)
    {
        var t = GetChildInChildren(transform, cameraName);
        if (t == null) {
            Debug.LogError("SPIELBERG ERROR: Camera " + cameraName + " does not exist or is not a CHILD of SpielbergAssistant");
        }
        SwitchCamera(t);
    }

    public void SwitchCamera(Transform newCamera)
    {
        Game.i.aperture.currentCamera.transform.parent = newCamera;
        if (newCamera != null) {
            Game.i.aperture.currentCamera.transform.localPosition = Vector3.zero;
            Game.i.aperture.currentCamera.transform.localRotation = Quaternion.identity;
        }
    }

    Transform GetChildInChildren(Transform t, string name)
    {
        if (t.gameObject.name == name) {
            return t;
        }
      
        for (int i = 0; i < t.childCount; i++) {
            var child = t.GetChild(i);
            var match = GetChildInChildren(child, name);
            if (match != null) return match;
        }

        return null;
    }

    public void PauseAndWaitForInput(EAction action)
    {
        director.Pause();
        waitingOnInput = action;
    }

    public void PauseAndWaitForEndOfDialogue()
    {
        director.Pause();
        isWaitingOnDialogue = true;
    }
}
