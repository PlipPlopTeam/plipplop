using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Pause : MonoBehaviour
{
    public bool paused;

    public GameObject pauseMenu;
    
    void Update()
    {
        if (Game.i.mapping.IsPressed(EAction.PAUSE))
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

        FindObjectOfType<EventSystem>().SetSelectedGameObject(pauseMenu.GetComponentInChildren<Button>().gameObject);
    }

    void StopPause()
    {
        Time.timeScale = 1;
        Time.fixedDeltaTime = .02f * Time.timeScale;
        paused = false;
        pauseMenu.SetActive(false);
    }

    public void Quit()
    {
        print("bisous");
        Application.Quit();
    }
}
