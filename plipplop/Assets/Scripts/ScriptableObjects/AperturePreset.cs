using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "AperturePreset", menuName = "ScriptableObjects/Aperture", order = 1)]
public class AperturePreset : ScriptableObject
{
    [System.Serializable]
    public class PropertyOverride
    {
        public string property;
        public bool isOverriden;
    }

    public List<PropertyOverride> overrides = new List<PropertyOverride>();

    [HideInInspector] public AperturePreset fallback;

    // Switches
    [HideInInspector] public bool canBeControlled = true;
    [HideInInspector] public bool canBeReset = true;
    [HideInInspector] public readonly bool detectsSurroundings = true;
    [HideInInspector] public LayerMask obstructibleLayerMask;

    // Basic parameters
    [HideInInspector] [Range(2f, 200f)] public float fieldOfView = 75f;
    [HideInInspector] public float heightOffset = 1f;
    [HideInInspector] public float maximumHeightAboveTarget = 10f;
    [HideInInspector] [Range(0f, 40f)] public float additionalAngle = 20f;
    [HideInInspector] public Aperture.Range distance = new Aperture.Range() { min = 5f, max = 8f };
    [HideInInspector] public float cameraRotateAroundSensivity = 4f;
	[HideInInspector] public float cameraRotateAboveSensivity = 2f;


	// Interpolations
	[HideInInspector] public float fovLerp = 1f;
    [HideInInspector] public float lateralFollowLerp = 1f;
    [HideInInspector] public float longitudinalFollowLerp = 1f;
    [HideInInspector] public float verticalFollowLerp = 10f;
    [HideInInspector] public float rotationSpeed = 1f;
    [HideInInspector] public float lookAtLerp = 4f;
    [HideInInspector] [Range(0.5f, 10f)] public float virtualTargetYCatchUp = 2f;

    // Align
    [HideInInspector] public float alignAfter = 2f;
	[HideInInspector] public float angleConsideredAlign = 10f;
	[HideInInspector] public float minTargetVelocity = 0.1f;
	[HideInInspector] public float alignMultiplierByUser = 2f;
	[HideInInspector] public float alignMultiplierByStick = 50f;
	[HideInInspector] public float alignMultiplierByVelocity = 2f;

    // Speed FX
    [HideInInspector] [Range(0f, 10)] public float speedEffectMultiplier = 1f;
    [HideInInspector] [Range(1f, 20f)] public float catchUpSpeedMultiplier = 1f;
    [HideInInspector] [Range(0f, 400f)] public float angleIncrementOnSpeed = 10f;

    // Advanced
    [HideInInspector] public float maximumCatchUpSpeed = 10f;
    [HideInInspector] public Aperture.Range absoluteBoundaries = new Aperture.Range() { min = 2f, max = 10f };
    [HideInInspector] public bool constraintToTarget = false;
    [HideInInspector] public Vector3 targetConstraintLocalOffset;
    [HideInInspector] public float staticPositionLerp = 1f;
    [HideInInspector] public float staticRotationLerp = 1f;
    [HideInInspector] public bool immediatePresetStackUpdate = false;

    public static AperturePreset CreateFromCamera(Camera cam)
    {
        var preset = CreateInstance<AperturePreset>();
        
        preset.fieldOfView = cam.fieldOfView;
        return preset;
    }
}