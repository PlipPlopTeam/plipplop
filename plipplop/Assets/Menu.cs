using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Random = UnityEngine.Random;

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

        StartCoroutine(Delay());

        gameCanvas = GameObject.Find("GAME_CANVAS");
        
        gameCanvas.SetActive(false);
        
        eventManager.SetSelectedGameObject(buttons[0].gameObject);
        selectedObject = eventManager.currentSelectedGameObject;
        
        SetButtonSelected(selectedObject);
        
        //Aperture.GetCurrentlyActiveCamera().Set
    }

    public void Play()
    {
        gameCanvas.SetActive(true);

       IntroQuest.BeginQuest();

        gameObject.SetActive(false);
        
        Game.i.player.Deparalyze();
        SoundPlayer.Play("sfx_pop_menu",.3f, Random.Range(.8f,1.2f));

    }

    public void Credits()
    {
        credits.SetActive(!credits.activeSelf);

        credits.transform.localPosition = creditsStartScale;
        credits.transform.DOPunchScale(Vector3.one / 2, .5f);
        
        SoundPlayer.Play("sfx_pop_menu",.3f, Random.Range(.8f,1.2f));
    }

    public void Quit()
    {
        SoundPlayer.Play("sfx_pop_menu",.3f, Random.Range(.8f,1.2f));

        Application.Quit();
    }

    private void Update()
    {
        if (selectedObject != eventManager.currentSelectedGameObject)
        {
            if (eventManager.currentSelectedGameObject != null)
            {

                if (selectedObject != null)
                {
                    RemoveButtonSelected(selectedObject);
                }


                selectedObject = eventManager.currentSelectedGameObject;

                if (selectedObject != null)
                {
                    SetButtonSelected(selectedObject);
                }
            }
            else
            {
                eventManager.SetSelectedGameObject(selectedObject);
            }
        }
    }

    void SetButtonSelected(GameObject _b)
    {
        TextMeshProUGUI _t = _b.GetComponentInChildren<TextMeshProUGUI>();
        if (_t)
        {
            SoundPlayer.Play("sfx_pop_menu",.3f, Random.Range(.8f,1.2f));

            _t.text += " <";
        }
    }

    void RemoveButtonSelected(GameObject _b)
    {
        TextMeshProUGUI _t = _b.GetComponentInChildren<TextMeshProUGUI>();
        if (_t)
        {
            string _s = _t.text;
            _s = _s.Remove(_s.Length - 2);
            _t.text = _s;
        }
    }

    IEnumerator Delay()
    {
        yield return null;
        
        Game.i.player.GetCurrentController().RetractLegs();
        Game.i.player.Paralyze();
    }
}
