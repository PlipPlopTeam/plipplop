using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pause : MonoBehaviour
{
    public bool paused;

    public GameObject pauseMenu;
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))//mettre input manette
        {
            if (paused)
            {
                StopPause();
            }
            else
            {
                StartPause();
            }
        }
    }



    void StartPause()
    {
        Time.timeScale = 0;
        Time.fixedDeltaTime = .02f * Time.timeScale;
        paused = true;
        
        pauseMenu.SetActive(true);
    }

    void StopPause()
    {
        Time.timeScale = 1;
        Time.fixedDeltaTime = .02f * Time.timeScale;
        paused = false;
        pauseMenu.SetActive(false);
    }
}
