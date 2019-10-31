﻿using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "AperturePreset", menuName = "ScriptableObjects/Aperture", order = 1)]
public class AperturePreset : ScriptableObject
{
    [HideInInspector] public Dictionary<string, bool> overrides = new Dictionary<string, bool>();

    public AperturePreset fallback;

    // Switches
    public bool canBeControlled = true;

    // Basic parameters
    [Range(2f, 200f)] public float fieldOfView = 75f;
    public float heightOffset;
    [Range(0f, 40f)] public float additionalAngle = 20f;
    public Aperture.Range distance;

    // Interpolations
    public float fovLerp = 1f;
    public float lateralFollowLerp = 1f;
    public float longitudinalFollowLerp = 1f;
    public float verticalFollowLerp = 10f;
    public float rotationSpeed = 1f;
    public float lookAtLerp = 4f;

    // Speed FX
    [Range(0.1f, 10)] public float speedEffectMultiplier = 1f;
    [Range(1f, 20f)] public float catchUpSpeedMultiplier = 1f;
    [Range(0f, 400f)] public float angleIncrementOnSpeed = 10f;

    // Advanced
    public float maximumCatchUpSpeed = 10f;
    public float cameraRotateAroundSpeed = 4f;
    public Aperture.Range absoluteBoundaries = new Aperture.Range() { min = 2f, max = 10f };
    public bool constraintToTarget = false;
    public Vector3 targetConstraintLocalOffset;
}