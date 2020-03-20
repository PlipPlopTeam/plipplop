using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrabDance : MonoBehaviour
{
    public List<Crab> crabs;

    public bool dancing;

    private Radio radio;

    public void StartDancing(bool _isRadio = true)
    {
        if (!dancing)
        {
            dancing = true;
            StartCoroutine(DanceDelay());
            
            SoundPlayer.Play("bgm_crab_rave", 1, 1, true);
        }
    }

    IEnumerator DanceDelay()
    {
        foreach (var _crab in crabs)
        {
            _crab.gameObject.SetActive(true);
            _crab.Pop();
            
            yield return new WaitForSecondsRealtime(.1f);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!radio)
        {
            radio = other.gameObject.GetComponent<Radio>();
        }

        if (radio)
        {
            if (radio.IsRadioOn())
            {
                StartDancing();
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (radio)
        {
            if (radio.IsRadioOn())
            {
                StartDancing();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (radio)
        {
            if (radio.gameObject == other.gameObject)
            {
                radio = null;
            }
        }
    }
}
