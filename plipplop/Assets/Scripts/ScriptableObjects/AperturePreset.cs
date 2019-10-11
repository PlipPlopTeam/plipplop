using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "AperturePreset", menuName = "ScriptableObjects/Aperture", order = 1)]
public class AperturePreset : ScriptableObject
{
    public Aperture.Settings settings;
}