using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "LocomotionPreset", menuName = "ScriptableObjects/Locomotion", order = 1)]
public class LocomotionPreset : ScriptableObject
{
    [Header("Locomotion")]
    public float maxSpeed = 800f;
    public float jump = 1200f;
    public AnimationCurve accelerationCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    [Header("Gravity")]
    public float baseDrag = 5f;
    public float strength = 1000f;
    public float maxFallSpeed = 40f;
}