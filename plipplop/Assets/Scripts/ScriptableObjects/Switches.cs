using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "SWITCHSETNAME", menuName = "ScriptableObjects/GameSwitches")]
public class Switches : ScriptableObject
{
    [SerializeField] [ReadOnlyInGame] private bool S_FADE_CHUNK_PROPS = false;
    public bool FADE_CHUNK_PROPS { get {return S_FADE_CHUNK_PROPS;} }
}
