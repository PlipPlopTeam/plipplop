using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "LocomotionPreset", menuName = "ScriptableObjects/Locomotion", order = 1)]
public class LocomotionPreset : ScriptableObject
{
    [Header("Locomotion")]
    public float speed = 70f;

    [Header("Gravity")]
    public float baseDrag = 15f;
    public float strength = 0.5f;
    public float maxFallSpeed = 10f;
}