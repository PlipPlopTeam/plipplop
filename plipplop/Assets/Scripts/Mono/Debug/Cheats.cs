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
    }

    public void ResetScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

}
