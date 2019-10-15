using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "LocomotionPreset", menuName = "ScriptableObjects/Locomotion", order = 1)]
public class LocomotionPreset : ScriptableObject
{
    [Header("Locomotion")]
    public float acceleration = 20f;
    public float speed = 200f;
    public float jump = 50f;

    [Header("Gravity")]
    public float baseDrag = 15f;
    public float strength = 0.5f;
    public float maxFallSpeed = 10f;
}