using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameFX", menuName = "ScriptableObjects/GameFX", order = 1)]
public class GameFX : ScriptableObject
{
    [System.Serializable]
    public class SFXParameter
    {
        public string name;
        public bool spatializedSound = false;
        public bool randomPitch = false;
        [Range(0f, 1f)] public float volume = 1f;
    }

    [Header("Sounds")]
    [Tooltip("List of sounds to play")]
    public bool randomSound = false;
    public List<SFXParameter> sfx;

    [Header("Effects")]
    [Tooltip("List of vfx to play")]
    public bool randomEffect = false;
    public List<string> vfx;
}
