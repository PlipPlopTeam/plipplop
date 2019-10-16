using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "LocomotionPreset", menuName = "ScriptableObjects/Locomotion", order = 1)]
public class LocomotionPreset : ScriptableObject
{
    [Header("Locomotion")]
    public float acceleration = 50000;
    public float speed = 800f;
    public float jump = 1200f;

    [Header("Gravity")]
    public float baseDrag = 5f;
    public float strength = 1000f;
    public float maxFallSpeed = 40f;
}