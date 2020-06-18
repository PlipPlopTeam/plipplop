using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticCameraVolume : Volume { 

    [Header("Specific options")]
    public Transform aim;
    public float FOV = 60f;
    public bool isImmediate = false;
    public bool lookAtTarget = false;

    public List<GameObject> objectsToHide = new List<GameObject>();

    bool isPlayerInside = false;
    int lookAtIndex = 0;
    Geometry.PositionAndRotation objective;
    Controller controller;

    public void Update()
    {
        var pc = Game.i.player.GetCurrentController();
        if (controller != null && pc != null && !IsInside(pc.transform.position))
        {
            OnPlayerExit(controller);
        }
        else if (controller == null && pc != null && IsInside(pc.transform.position)) {
            OnPlayerEnter(pc);
        }
    }

    public override void OnPlayerEnter(Controller player)
    {
        if (isPlayerInside) return;
        isPlayerInside = true;

        objective = Game.i.aperture.AddStaticPosition(aim);

        if (isImmediate){
            Game.i.aperture.FixedUpdate();
            Game.i.aperture.fieldOfView.destination = FOV;
            Game.i.aperture.Teleport();
            Game.i.aperture.Freeze();
        }

        if (!lookAtTarget) 
        {
            lookAtIndex = Game.i.aperture.DisableLookAt();
        }
        else 
        {
            Game.i.aperture.EnableLookAt();
        }

        // Stop control for 1 frame
        StartCoroutine(FreezeForSomeTime());

        if (objectsToHide != null) {
            foreach (GameObject o in objectsToHide) {
                if (o == null) continue;
                var fade = o.GetComponent<FadedApparition>();
                if (fade != null) {
                    fade.FadeOutThen(null);
                }
                else {
                    o.SetActive(false);
                }
            }
        }
        controller = player;
    }

    public override void OnPlayerExit(Controller player)
    {
        if (!isPlayerInside) return;
        isPlayerInside = false;

        if (isImmediate) Game.i.aperture.Unfreeze();

        Game.i.aperture.RemoveStaticPosition(objective);
        
        if (!lookAtTarget && lookAtIndex > -1)
        {
            Game.i.aperture.RestoreLookAt(lookAtIndex);
        }

        StartCoroutine(FreezeForSomeTime());

        if (objectsToHide != null)
        {
            foreach (GameObject o in objectsToHide)
            {
                if (o != null)
                {
                    var fade = o.GetComponent<FadedApparition>();
                    if (fade != null)
                    {
                        fade.StartFadeIn();
                    }
                    else
                    {
                        o.SetActive(true);
                    }
                }
            }
        }

        controller = null;
    }

    IEnumerator FreezeForSomeTime()
    {

        Game.i.player.Paralyze();
        yield return new WaitForSeconds(0.2f);
        Game.i.player.Deparalyze();
    }
}
