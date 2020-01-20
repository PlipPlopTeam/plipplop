using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameFX", menuName = "ScriptableObjects/GameFX", order = 1)]
public class GameFX : ScriptableObject
{
    [Tooltip("List of sounds to play")]
    public List<string> sfx;
    [Tooltip("List of vfx to play")]
    public List<string> vfx;
}
