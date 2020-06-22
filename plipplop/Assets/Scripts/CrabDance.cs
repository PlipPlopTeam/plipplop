using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrabDance : MonoBehaviour
{
    public List<Crab> crabs;

    public int bpm = 90;

    public bool areDancing { private set; get; }

    private void Awake()
    {
        foreach(var crab in crabs) {
            crab.isStatic = true;
        }
    }

    [ContextMenu("Dance")]
    public void StartDancing()
    {
        if (!areDancing)
        {
            areDancing = true;
            StartCoroutine(DanceDelay());
            
            SoundPlayer.PlayAtPosition("bgm_crab_rave", transform.position, .2f, false, true);
        }
    }

    IEnumerator DanceDelay()
    {
        while (true) {
            foreach (var _crab in crabs) {
                _crab.gameObject.SetActive(true);
                if (_crab.hidden) {
                    _crab.Show();
                }
                else {
                    _crab.Hide(true);
                }

                yield return new WaitForSeconds(.02f);
            }
            yield return new WaitForSeconds(60f/bpm - .02f * crabs.Count);
        }
    }
}
