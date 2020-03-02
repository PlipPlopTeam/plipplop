using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrabDance : MonoBehaviour
{
    public List<Crab> crabs;

    public bool dancing;

    public void StartDancing()
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
        Radio _radio = other.gameObject.GetComponent<Radio>();

        if (_radio)
        {
            if (_radio.IsRadioOn())
            {
                StartDancing();
            }
        }
    }
}
