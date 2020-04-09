using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TalkingSphere : TalkableCharacter
{

    public override Dialog OnDialogTrigger()
    {
        return Game.i.library.dialogs["test"];
    }

    public override byte[] Save()
    {
        return new byte[] { };
    }

    public override void Load(byte[] data)
    {

    }
}
