using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Cheats : Dictionary<string, System.Action> 
{
    public Cheats()
    {
        Add("RESET", ResetScene);
        Add("ZIK", delegate { SoundPlayer.Play("bgm_test"); });
        Add("FAMINE", SetAllHunderToHundred);
    }

    public void ResetScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void SetAllHunderToHundred()
    {
        foreach(NonPlayableCharacter npc in Object.FindObjectsOfType<NonPlayableCharacter>())
            npc.SetStat(NonPlayableCharacter.EStat.HUNGER, 100f);
    }
}
