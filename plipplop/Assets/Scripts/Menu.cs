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
    public EventSystem eventManager;
    public List<Button> buttons;
    
    GameObject selectedObject;
    Vector3 creditsStartScale;
    new Camera camera;

    private AudioSource menuAudioSource;
    
    private void Start()
    {
        menuAudioSource = SoundPlayer.PlaySoundAttached("bgm_gymtonic", transform, .05f,false,true);

        
        creditsStartScale = credits.transform.localScale;
        
        //ajouter ouverture de l'écran pour laisser la scene se charger
        //Game.i.Transition();

        StartCoroutine(Delay());

        gameCanvas = GameObject.Find("GAME_CANVAS");
        
        gameCanvas.GetComponentInChildren<InputDisplay>().HideInputs();

        gameCanvas.SetActive(false);
        
        eventManager.SetSelectedGameObject(buttons[0].gameObject);
        selectedObject = eventManager.currentSelectedGameObject;
        
        SetButtonSelected(selectedObject);

        // Camera work
        camera = GetComponent<Camera>();
        camera.enabled = false;

        Game.i.aperture.Freeze();
        Game.i.aperture.currentCamera.transform.parent = transform;
    }

    public void Play()
    {
        SoundPlayer.StopSound(menuAudioSource, true);

        foreach (Radio _r in FindObjectsOfType<Radio>())
        {
            if (_r.musics.Count > 0)
            {
                _r.PlayMusic();
            }
        }
        
        gameCanvas.SetActive(true);

        Game.i.player.Deparalyze();
        Game.i.aperture.Unfreeze();

        Spielberg.PlayCinematic("cine_intro");
        Spielberg.onCinematicEnded += LaunchQuestAfterCinematic;

        SoundPlayer.Play("sfx_pop_menu", .3f, Random.Range(.8f, 1.2f));
        gameObject.SetActive(false);
    }

    void LaunchQuestAfterCinematic()
    {
        IntroQuest.BeginQuest();
        Spielberg.onCinematicEnded -= LaunchQuestAfterCinematic;
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
        Game.i.aperture.currentCamera.transform.localPosition = Vector3.zero;
        Game.i.aperture.currentCamera.transform.localRotation = Quaternion.identity;
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
