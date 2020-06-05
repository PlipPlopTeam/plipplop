using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MwonMwonIntroQuest : TalkableCharacter
{
    public Controller tutorialController;
    public GameObject DBG_FirstCinematic; // DEBUG
    public GameObject DBG_SecondCinematic; // DEBUG
    public GameObject stopGap;

    public Launcher launcher;
    public CollisionEventTransmitter exitVolume;

    bool hasLaunchedCinematicPartTwo = false;
    bool hasFinishedTutorial = false;
    float radius = 0f;

    public override Dialog OnDialogTrigger()
    {
        return null;
    }

    private void Start()
    {
        radius = talkRadius;
        talkRadius = 0f;
        if (DBG_FirstCinematic) Spielberg.PlayCinematic(DBG_FirstCinematic);
        else Spielberg.PlayCinematic("cine_firststeps_introduction");

        exitVolume.onTriggerEnter += ExitVolume_onTriggerEnter;
    }

    private void ExitVolume_onTriggerEnter(Collider obj)
    {
        var controller = Game.i.player.GetCurrentController();
        if (controller!= null && obj.gameObject == controller.gameObject) {
            launcher.LaunchController(controller);
        }
    }

    new void Update()
    {
        base.Update();
        if (hasLaunchedCinematicPartTwo) {
            if (Game.i.player.IsPossessingBaseController() && !hasFinishedTutorial) {
                hasFinishedTutorial = true;
            }
        }
        else{
            if (Game.i.player.IsPossessing(tutorialController)) {
                if (DBG_SecondCinematic) Spielberg.PlayCinematic(DBG_SecondCinematic);
                else Spielberg.PlayCinematic("cine_firststeps_aftermorphin");
                hasLaunchedCinematicPartTwo = true;
            }
        }
        
        if (hasFinishedTutorial) {
            talkRadius = radius;
            stopGap.SetActive(false);
        }
        sphereTrigger.radius = talkRadius;
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
