using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "LocomotionPreset", menuName = "ScriptableObjects/Locomotion", order = 1)]
public class LocomotionPreset : ScriptableObject
{
    [Header("Locomotion")]
    public float maxSpeed = 800f;
    public float jump = 1200f;
    public AnimationCurve accelerationCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
    [Range(0f, 1f)] public float groundFriction = 0.15f;
    [Range(0f, 1f)] public float airFriction = 0f;
    public PhysicMaterialCombine frictionCombine = PhysicMaterialCombine.Minimum;
    [Range(0f, 100f)] public float airControl = 65f;
    [Range(0f, 100f)] public float waterControl = 65f;
    [Range(0f, 2f)] public float waterSpeedFactor = 0.6f;
    [Range(0f, 100f)] public float maxWalkableSteepness = 50f;
    [Range(0f, 1f)] public float groundedBufferToleranceSeconds = 0.2f;
    [Range(2f, 50f)] public float lookForwardSpeed = 8f;
}