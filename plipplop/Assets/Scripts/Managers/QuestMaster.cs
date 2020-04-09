using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestMaster
{
    List<TalkableCharacter> quests = new List<TalkableCharacter>();

    public void Register(TalkableCharacter quest)
    {
        // Load here if payload data is available for this quest
        /*
       
        if (dataIsAvailableFor(quest)){
            quest.Load(data);
        }
        
        */
        if (!quests.Find(o=>o.questUniqueId == quest.questUniqueId)) quests.Add(quest);
    }
}
