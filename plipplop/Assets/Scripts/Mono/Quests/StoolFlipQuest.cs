using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoolFlipQuest : TalkableCharacter
{
    public GameObject stuntManHelmet;

    bool hasStarted = false;
    bool isFinishedStepOne = false;
    bool isFinishedStepTwo = false;
    int comeBacks = 0;
    int flipsForStepOne = 1;
    int flipsForStepTwo = 1;

    int flips = 0;

    public override Dialog OnDialogTrigger()
    {
        var dials = Game.i.library.dialogs;

        if (!hasStarted) {
            hasStarted = true;
            return dials.Get("stuntman_quest_beginning");
        }
        else {
            if (isFinishedStepTwo) {
                return dials.Get("stuntman_quest_end");
            }
            else if (isFinishedStepOne){
                if (comeBacks == 0) {
                    return dials.Get("stuntman_quest_firststunt");
                }
                else {
                    return dials.Get("stuntman_quest_benchclue");
                }
            }
            else {
                if (comeBacks == 0) {
                    comeBacks++;
                    return dials.Get("stuntman_quest_firstcomeback");
                }
                else {
                    return dials.Get("stuntman_quest_secondcomeback");
                }
            }

        }
    }

    new void Update()
    {
        base.Update();
        var pc = Game.i.player.GetCurrentController();
        if (!isFinishedStepOne && pc is Stool) {
            var controller = (Stool)pc;
            if (flips < controller.flips) {
                flips = controller.flips;
            }
            if (flips >= flipsForStepOne) {
                isFinishedStepOne = true;

                flips = 0;
            }
        }
        else if (isFinishedStepOne && !isFinishedStepTwo && pc is Bench) {
            var controller = (Bench)pc;
            if (flips < controller.flips) {
                flips = controller.flips;
            }
            if (flips >= flipsForStepTwo) {
                isFinishedStepTwo = true;
                flips = 0;
                AddHat();
            }
        }
        else {
            flips = 0;
        }
    }

    void AddHat()
    {
        GameObject _mouette = Instantiate(stuntManHelmet, Game.i.player.baseController.visuals.transform);
        _mouette.transform.localPosition = new Vector3(0, 0, 0);
        _mouette.transform.localEulerAngles = new Vector3(0, 0, 0);
    }

    public override void Load(byte[] data)
    {
        isFinishedStepOne = Convert.ToBoolean(data[0]);
        isFinishedStepTwo = Convert.ToBoolean(data[1]);
        hasStarted = Convert.ToBoolean(data[2]);
    }

    public override byte[] Save()
    {
        return new byte[] { 
            Convert.ToByte(isFinishedStepOne),
            Convert.ToByte(isFinishedStepTwo),
            Convert.ToByte(hasStarted)
        };
    }
}
