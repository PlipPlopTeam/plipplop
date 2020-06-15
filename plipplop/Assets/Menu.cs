using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Menu : MonoBehaviour
{
    
    public List<GameObject> introScene;

    public GameObject credits;

    public MwonMwonIntroQuest IntroQuest;
    
    public GameObject gameCanvas;

    private Vector3 creditsStartScale;
    private void Start()
    {
        creditsStartScale = credits.transform.localScale;
        
        //ajouter ouverture de l'écran pour laisser la scene se charger
        //Game.i.Transition();
        
        Game.i.player.Paralyze();

        gameCanvas = GameObject.Find("GAME_CANVAS");
        
        gameCanvas.SetActive(false);
    }

    public void Play()
    {
        gameCanvas.SetActive(true);

       IntroQuest.BeginQuest();

        gameObject.SetActive(false);
    }

    public void Credits()
    {
        credits.SetActive(!credits.activeSelf);

        credits.transform.localPosition = creditsStartScale;
        credits.transform.DOPunchScale(Vector3.one / 2, .5f);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
