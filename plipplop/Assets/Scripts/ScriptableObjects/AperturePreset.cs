using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "AperturePreset", menuName = "ScriptableObjects/Aperture", order = 1)]
public class AperturePreset : ScriptableObject
{
    [HideInInspector] public Dictionary<string, bool> overrides = new Dictionary<string, bool>();

    [HideInInspector] public AperturePreset fallback;

    // Switches
    [HideInInspector] public bool canBeControlled = true;

    // Basic parameters
    [HideInInspector] [Range(2f, 200f)] public float fieldOfView = 75f;
    [HideInInspector] public float heightOffset;
    [HideInInspector] [Range(0f, 40f)] public float additionalAngle = 20f;
    [HideInInspector] public Aperture.Range distance;

    // Interpolations
    [HideInInspector] public float fovLerp = 1f;
    [HideInInspector] public float lateralFollowLerp = 1f;
    [HideInInspector] public float longitudinalFollowLerp = 1f;
    [HideInInspector] public float verticalFollowLerp = 10f;
    [HideInInspector] public float rotationSpeed = 1f;
    [HideInInspector] public float lookAtLerp = 4f;

    // Speed FX
    [HideInInspector] [Range(0.1f, 10)] public float speedEffectMultiplier = 1f;
    [HideInInspector] [Range(1f, 20f)] public float catchUpSpeedMultiplier = 1f;
    [HideInInspector] [Range(0f, 400f)] public float angleIncrementOnSpeed = 10f;

    // Advanced
    [HideInInspector] public float maximumCatchUpSpeed = 10f;
    [HideInInspector] public float cameraRotateAroundSpeed = 4f;
    [HideInInspector] public Aperture.Range absoluteBoundaries = new Aperture.Range() { min = 2f, max = 10f };
    [HideInInspector] public bool constraintToTarget = false;
    [HideInInspector] public Vector3 targetConstraintLocalOffset;
}