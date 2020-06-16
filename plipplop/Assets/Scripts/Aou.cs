using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Aou : TalkableCharacter
{
    public List<string> QuestIds;

    public int questIdIndex;
    
    public override Dialog OnDialogTrigger()
    {
        Dialog _d = Game.i.library.dialogs[QuestIds[questIdIndex]];
        
        questIdIndex++;

        if (questIdIndex >= QuestIds.Count)
        {
            questIdIndex = 0;
        }

        return _d;
    }

    public override byte[] Save()
    {
        return new byte[] { };
    }

    public override void Load(byte[] data)
    {

    }
    
    
}
