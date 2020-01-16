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
        Add("GLUE", delegate { Game.i.player.Paralyze(); });
        Add("FREE", delegate { Game.i.player.Deparalyze(); });
        Add("FAMINE", SetAllHunderToHundred);
        Add("HELLO", SpawnNPC);
        Add("IMONFIRE", SpawnFire);
        Add("BOOM", SpawnPoof);
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

    public void SpawnNPC()
    {
        var o = Object.Instantiate(Game.i.library.npcLibrary.NPCSamplePrefab);
        o.transform.position = Game.i.player.GetCurrentController().transform.position + Game.i.player.GetCurrentController().transform.forward * 3f;
    }

    public void SpawnPoof()
    {
        Pyromancer.Play("poof", Game.i.player.GetCurrentController().transform.position);
    }

    public void SpawnFire()
    {
        Pyromancer.PlayAttached("fire", Game.i.player.GetCurrentController().transform);
    }
}
