using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    
    public List<GameObject> introScene;

    public GameObject credits;

    public MwonMwonIntroQuest IntroQuest;
    
    public GameObject gameCanvas;

    private Vector3 creditsStartScale;

    public EventSystem eventManager;

    public List<Button> buttons;
    
    private GameObject selectedObject;
    
    private void Start()
    {
        creditsStartScale = credits.transform.localScale;
        
        //ajouter ouverture de l'écran pour laisser la scene se charger
        //Game.i.Transition();
        
        Game.i.player.Paralyze();

        gameCanvas = GameObject.Find("GAME_CANVAS");
        
        gameCanvas.SetActive(false);
        
        eventManager.SetSelectedGameObject(buttons[0].gameObject);
        selectedObject = eventManager.currentSelectedGameObject;
        
        SetButtonSelected(selectedObject);
    }

    public void Play()
    {
        gameCanvas.SetActive(true);

       IntroQuest.BeginQuest();

        gameObject.SetActive(false);
        
        Game.i.player.Deparalyze();
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

    private void Update()
    {
        if (selectedObject != eventManager.currentSelectedGameObject)
        {
            RemoveButtonSelected(selectedObject);
            selectedObject = eventManager.currentSelectedGameObject;
            SetButtonSelected(selectedObject);
        }
    }

    void SetButtonSelected(GameObject _b)
    {
        _b.GetComponentInChildren<TextMeshProUGUI>().text += " <";
    }

    void RemoveButtonSelected(GameObject _b)
    {
        TextMeshProUGUI _t = _b.GetComponentInChildren<TextMeshProUGUI>();
        string _s = _t.text;
        _s = _s.Remove(_s.Length - 2);
        _t.text = _s;
    }
}
