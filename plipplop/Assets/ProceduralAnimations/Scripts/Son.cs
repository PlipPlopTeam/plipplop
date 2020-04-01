using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Son : MonoBehaviour
{
    public static Son I;

    public List<AudioClip> sons;

    private void Awake()
    {
        I = this;
    }

    public void PlaySound(int _index, bool _pitch = true, float _volume = .5f)
    {
        GameObject _o = new GameObject();
        AudioSource _son = _o.AddComponent<AudioSource>();
        _son.clip = sons[_index];
        if (_pitch)
        {
            _son.pitch = Random.Range(.9f, 1.1f);
        }

        _son.volume = _volume;
        
        _son.Play();
        
        Destroy(_o,_son.clip.length);
    }
}
