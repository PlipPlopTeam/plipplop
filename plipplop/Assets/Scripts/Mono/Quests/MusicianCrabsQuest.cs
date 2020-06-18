using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicianCrabsQuest : TalkableCharacter
{
    public CrabDance crabDance;


    bool hasPlayerTheRightController { get {
            return Game.i.player.GetCurrentController() != null && Game.i.player.GetCurrentController() is Bucket;
        } }

    int getPlayerHeldItems { get {
            return ((Bucket)Game.i.player.GetCurrentController()).itemCount;
        } }

    bool hasAlreadyComeWithZeroShell = false;
    bool isFirstTime = true;
    byte isFinished = 0;

    public override Dialog OnDialogTrigger()
    {
        var dials = Game.i.library.dialogs;

        if (isFirstTime) {
            isFirstTime = false;
            return dials.Get("unlock the quest_claw's comeback");
        }
        else {
            if (hasPlayerTheRightController) {
                switch (getPlayerHeldItems) {
                    case 0:
                        if (hasAlreadyComeWithZeroShell) {
                            return dials.Get("first time with 0 shell_claw's comeback");
                        }
                        else {
                            return dials.Get("X time with 0 shell_claw's comeback");
                        }

                    case 1:
                    case 2:
                    case 3:
                    case 4:
                        return dials.Get("X time with " + getPlayerHeldItems + " shell_claw's comeback");

                    case 5:
                    default:
                        isFinished = 1;
                        return dials.Get("resolve the quest_claw's comeback_before the dance");
                }
            }
            else {
                return dials.Get("back with wrong controller_claw's comeback");
            }
        }
    }

    new private void Update()
    {
        base.Update();
        if (isFinished > 0 && !Game.i.player.IsParalyzed() && !crabDance.areDancing) {
            crabDance.StartDancing();
        }
    }

    public override void Load(byte[] data)
    {
        isFinished = data[0];
    }

    public override byte[] Save()
    {
        return new byte[] { isFinished };
    }
}

