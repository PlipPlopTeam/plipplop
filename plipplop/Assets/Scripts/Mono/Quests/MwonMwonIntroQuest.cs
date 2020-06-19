using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using SpielbergClips;
using UnityEngine;

public class MwonMwonIntroQuest : TalkableCharacter
{
    public Controller tutorialController;
    public GameObject DBG_FirstCinematic; // DEBUG
    public GameObject DBG_SecondCinematic; // DEBUG
    public GameObject stopGap;
    public StaticCameraVolume scm;
    public Launcher launcher;
    public CollisionEventTransmitter exitVolume;
    
    

    bool hasLaunchedCinematicPartTwo = false;
    bool hasFinishedTutorial = false;
    float radius = 0f;
    
    bool stopgapRemoved;

    public AnimationCurve rumbleCurve;
    public AnimationCurve explosionCurve;

    public ParticleSystem splashPS;

    public override Dialog OnDialogTrigger()
    {
        var lib = Game.i.library.dialogs;
        if (Game.i.player.IsPossessingBaseController()) {
            return lib["collection_nude"];
        }
        else if (Game.i.player.GetCurrentController() is Ball) {
            return lib["collection_complete"];
        }
        else {
            return lib["collection_wrong_item"];
        }
    }

    public void BeginQuest()
    {
        radius = talkRadius;
        talkRadius = 0f;
        if (DBG_FirstCinematic) Spielberg.PlayCinematic(DBG_FirstCinematic);
        else Spielberg.PlayCinematic("cine_tutorial_1");

        exitVolume.onTriggerEnter += ExitVolume_onTriggerEnter;
        
        SoundPlayer.PlayAtPosition("bgm_volcano", transform.position,.5f);
    }

    private void ExitVolume_onTriggerEnter(Collider obj)
    {
        var controller = Game.i.player.GetCurrentController();
        if (controller!= null && obj.gameObject == controller.gameObject)
        {
            scm.OnPlayerExit(controller);
            launcher.LaunchController(controller);
        }
    }

    new void Update()
    {
        base.Update();

        if (hasFinishedTutorial) {
            if (!stopgapRemoved) {
                stopgapRemoved = true;
                StartCoroutine(StopGapAnim());
            }
        }

        if (hasLaunchedCinematicPartTwo) {
            if (Game.i.player.IsPossessingBaseController() && !hasFinishedTutorial) {
                hasFinishedTutorial = true;
            }
        }
        else{
            if (Game.i.player.IsPossessing(tutorialController)) {
                if (DBG_SecondCinematic) Spielberg.PlayCinematic(DBG_SecondCinematic);
                else Spielberg.PlayCinematic("cine_tutorial_2");
                hasLaunchedCinematicPartTwo = true;
            }
        }
        sphereTrigger.radius = talkRadius;
    }

    IEnumerator StopGapAnim()
    {
        while (DialogHooks.currentInterlocutor != null) {
            yield return new WaitForEndOfFrame();
        }

        //Play rumbble sound
        //vibrer manette

        Vector3 _pos = stopGap.transform.position;
        Vector3 _rot = stopGap.transform.localEulerAngles;
        
        float _timer = 0;
        
        while (_timer < 1)
        {
            stopGap.transform.position = _pos + rumbleCurve.Evaluate(_timer)*UnityEngine.Random.insideUnitSphere / 10;
            stopGap.transform.localEulerAngles = _rot + rumbleCurve.Evaluate(_timer)*UnityEngine.Random.insideUnitSphere;

            Game.i.aperture.Shake(rumbleCurve.Evaluate(_timer)*.07f,rumbleCurve.Evaluate(_timer)*2,1);
            
            _timer += Time.deltaTime/1.5f;
            yield return null;
        }

        _timer = 0;
        
        if(splashPS != null) splashPS.Play();

        while (_timer < 1)
        {
            Game.i.aperture.Shake(explosionCurve.Evaluate(_timer)*.125f,explosionCurve.Evaluate(_timer)*3,1);
            stopGap.transform.position = Vector3.Lerp(stopGap.transform.position, _pos + Vector3.up * 7, _timer);
            _timer += Time.deltaTime * 3;
            yield return null;
        }
        //Play splash lave vfx
        
        //stopGap.transform.DOMove(stopGap.transform.position + Vector3.up * 7, .4f);

        
        //play popsound
        
        yield return new WaitForSecondsRealtime(.4f);
        stopGap.SetActive(false);
        talkRadius = radius;
    }

    public override void Load(byte[] data)
    {
        throw new System.NotImplementedException();
    }

    public override byte[] Save()
    {
        throw new System.NotImplementedException();
    }
}
