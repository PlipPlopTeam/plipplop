using System.Collections.Generic;
using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using static Geometry;

public class Aperture
{
    public class Key<T>
    {
        public T destination;
        public T current;

        public Key() { }
        public Key(T value)
        {
            destination = value;
            current = value;
        }

        public void SetToDestination()
        {
            current = destination;
        }
    }

    public class StaticObjective
    {
        public PositionAndRotation positionAndRotation;
        public float? manualLerp;

        public StaticObjective(PositionAndRotation pAR)
        {
            positionAndRotation = pAR;
        }
    }

    [Serializable]
    public class Range
    {
        public float min;
        public float max;

        public static Range Lerp(Range a, Range b, float delta)
        {
            return new Range() { min = Mathf.Lerp(a.min, b.min, delta), max = Mathf.Lerp(a.max, b.max, delta) };
        }
    }

    public Camera currentCamera { get { return cam; } }

    public Key<float> fieldOfView = new Key<float>();
    public Key<Vector3> position = new Key<Vector3>();
    public Key<Quaternion> rotationAroundTarget = new Key<Quaternion>(Quaternion.identity);

    Transform target;
    Key<Vector3> virtualTarget = new Key<Vector3>();
    Vector3 defaultTarget;

    AperturePreset settings;
    List<AperturePreset> presetStack = new List<AperturePreset>();
    float stackUpdateSpeed = 0.7f;
    float stackTransitionState = 1f;
    AperturePreset previousStackSettings;
    Camera cam;

    // Static cameras
    List<StaticObjective> staticObjectives = new List<StaticObjective>();
    List<bool> lookAtTarget = new List<bool>();

    // SPEED
    Vector3 lastTargetPosition;
    float hDistanceToTarget = 0f;
    float destinationReachedThreshold = 0f;
    float rotationReachedThreshold = 5f;

    // SHAKE
    Vector3 shakeDisplacement = Vector3.zero;
    float shakeTimeRemaining = 0f;
    float shakeForce = 0f;
    int updateEvery = 2;
    int lastUpdate = 0;

    // Current angle on Y axis
    float hAngle;
	float hAngleAmount;
	// Current angle on X axis
	float vAngle;
    float vAngleAmount;
	float rotationMultiplier;
	bool isCameraBeingRepositioned;
	bool isCameraIdle = false;
	bool isTargetMoving = false;
    bool isTryingToRealignManually = false;
    float lastCameraInput;

    bool freeze = false;
    public void Freeze() {freeze = true;}
    public void Unfreeze() {freeze = false;}

    public void Load(AperturePreset s)
    {
        DestroyPreviousStackIfTemporary();
        if (presetStack.Count == 0) {
            previousStackSettings = s;
        }
        else {
            previousStackSettings = stackTransitionState < 1f ? ComputeSettings() : presetStack[presetStack.Count - 1];
            stackTransitionState = 0f;
        }
        presetStack.Add(s);
    }

    public void SwapLastTwoStackElements()
    {
        var t = presetStack[presetStack.Count - 1];
        presetStack[presetStack.Count - 1] = presetStack[presetStack.Count - 2];
        presetStack[presetStack.Count - 2] = t;
    }

    public void Unload(AperturePreset s)
    {
        DestroyPreviousStackIfTemporary();
        previousStackSettings = stackTransitionState < 1f ? ComputeSettings() : presetStack[presetStack.Count - 1];
        stackTransitionState = 0f;
        presetStack.Remove(s);
    }

    public void UnloadLast()
    {
        DestroyPreviousStackIfTemporary();
        previousStackSettings = stackTransitionState < 1f ? ComputeSettings() : presetStack[presetStack.Count - 1];
        stackTransitionState = 0f;
        presetStack.RemoveAt(presetStack.Count - 1);
    }

    public void DestroyPreviousStackIfTemporary()
    {
        if (presetStack.Count > 0 && previousStackSettings != null && !presetStack.Find(o => o == previousStackSettings) && previousStackSettings != Game.i.library.defaultAperture) {
            UnityEngine.Object.Destroy(previousStackSettings);
        }
    }

    public AperturePreset GetSettings()
    {
        return settings;
    }

    public AperturePreset ComputeSettings()
    {
        if (presetStack.Count < 1) {
            return UnityEngine.Object.Instantiate(Game.i.library.defaultAperture);
        }

        var preset = ScriptableObject.CreateInstance<AperturePreset>();
        preset.fieldOfView = Mathf.Lerp(previousStackSettings.fieldOfView, presetStack[presetStack.Count - 1].fieldOfView, stackTransitionState);
        preset.absoluteBoundaries = Range.Lerp(previousStackSettings.absoluteBoundaries, presetStack[presetStack.Count - 1].absoluteBoundaries, stackTransitionState);

        foreach (var property in typeof(AperturePreset).GetFields()) {
            property.SetValue(preset, property.GetValue(stackTransitionState > 0.5F ? presetStack[presetStack.Count - 1] : previousStackSettings));
        }

        return preset;
    }

    public Aperture()
    {
        cam = Camera.main ?? new GameObject().AddComponent<Camera>();
        cam.gameObject.name = "_CAMERA";
        previousStackSettings = Game.i.library.defaultAperture;

        stackTransitionState = 1f;
        settings = ComputeSettings();
    }

    public Aperture(AperturePreset s)
    {
        cam = Camera.main;
        Load(s);
    }

    public bool IsTransitioningOnStack()
    {
        return stackTransitionState < 1f;
    }

    public int DisableLookAt()
    {
        lookAtTarget.Add(false);
        return lookAtTarget.Count - 1;
    }

    public void RestoreLookAt(int index)
    {
        lookAtTarget.RemoveAt(index);
    }

    public int EnableLookAt()
    {
        lookAtTarget.Add(true);
        return lookAtTarget.Count - 1;
    }

    public bool IsLookAtEnabled()
    {
        return lookAtTarget.Count == 0 || lookAtTarget[lookAtTarget.Count - 1];
    }

    public Vector3 Forward()
    {
        return new Vector3(cam.transform.forward.x, 0f, cam.transform.forward.z).normalized;
    }

    public Vector3 Right()
    {
        return new Vector3(cam.transform.right.x, 0f, cam.transform.right.z).normalized;
    }

    public void RotateWithGamepad(float x = 0f, float y = 0f, float z = 0f)
    {
        if (settings.canBeControlled) Rotate(x, y, z);
    }

    public void Rotate(Vector3 rot)
    {
		hAngleAmount += rot.y * settings.cameraRotateAroundSensivity * Time.deltaTime;
        vAngleAmount = Mathf.Lerp(vAngleAmount, Mathf.Lerp(vAngleAmount, rot.x, settings.cameraRotateAboveSensivity * Time.deltaTime), Mathf.Abs(rot.x));

        if (rot.magnitude > 0f)
		{
			ResetIdleTime();
		}
    }
    public void Rotate(float x = 0f, float y = 0f, float z = 0f)
    {
        Rotate(new Vector3(x, y, z));
    }

    public void Update()
    {
		if (Time.time - lastCameraInput > settings.alignAfter && !isCameraIdle)
		{
			isCameraIdle = true;
			Realign();
		}
	}

	public void UserAlign()
	{
		if(settings.canBeReset)
		{
            isTryingToRealignManually = true;
            Realign();
		}
	}

    public void DeclareUserNotAligning()
    {
        isTryingToRealignManually = false;
    }

    public bool IsUserAligning()
    {
        return isTryingToRealignManually;
    }

    public void Realign()
    {
		rotationMultiplier = settings.alignMultiplierByUser;
		isCameraBeingRepositioned = true;
    }
	public void DeclareAligned()
	{
		rotationMultiplier = 1f;
		isCameraBeingRepositioned = false;
		ResetIdleTime();
	}

	public void ResetIdleTime()
	{
		isCameraBeingRepositioned = false;
		isCameraIdle = false;
		lastCameraInput = Time.time;
	}

    public bool IsCameraBeingRepositioned()
    {
        return isCameraBeingRepositioned;
    }

	public void FixedUpdate()
	{
		if (freeze) return;

        GameObject.Destroy(settings);
        settings = ComputeSettings();


        if (stackTransitionState < 1f) {
            stackTransitionState += Time.deltaTime * stackUpdateSpeed;
        }


        if (settings.constraintToTarget)
		{
			cam.transform.parent = target;
			cam.transform.localPosition = settings.targetConstraintLocalOffset;
			cam.transform.forward = target.forward;
            return;
		}
		else cam.transform.parent = null;

        if (target) UpdateVirtualTarget();

		var targetPosition = target ? virtualTarget.current : defaultTarget;
		var targetMovementVelocity = Vector3.Distance(targetPosition, lastTargetPosition);

        isTargetMoving = false;

        if (targetMovementVelocity > settings.minTargetVelocity) {
            isTargetMoving = true;
        }

		// If that bool is true, the camera will immediatly put itself in the back of the player
		if (target != null && isCameraBeingRepositioned)
		{

			hAngle = Vector3.SignedAngle(Vector3.back, target.forward, Vector3.up);
			float a = Vector3.SignedAngle(Forward(), target.forward, Vector3.up);
			if (Mathf.Abs(a) < settings.angleConsideredAlign) 
                DeclareAligned();

        }
        else {
            hAngle += hAngleAmount;
        }

        if (isTargetMoving) {
            ResetIdleTime();
            // Target moving and user not touching the pad, I will put myself in the back of the player
            if (Mathf.Abs(hAngleAmount) <= 0f) {
                float currentAngle = Vector3.SignedAngle(Forward(), target.forward, Vector3.up);
                float cameraTurnMultiplier = 1f - Mathf.Abs(currentAngle / 180f);

                // Camera trying to get in my back
                if (targetMovementVelocity > 0 || isCameraBeingRepositioned) {
                    hAngle += cameraTurnMultiplier * currentAngle * Time.fixedDeltaTime;
                }
            }
        }

        hAngle = hAngle % 360f;
        hAngleAmount = 0f;

        float vAngleAmplitude = 40f - settings.additionalAngle;
		vAngle = vAngleAmount * vAngleAmplitude;

        // Calculating "catch up"
        ComputeHorizontalDistanceToTarget(targetPosition);
		float catchUpSpeed = GetCatchUpSpeed();

		// Rotation
		ComputeRotation();
		UpdateRotation();

		// Position

		ComputePosition(targetPosition);
		UpdatePosition(catchUpSpeed);
		if (GetStaticObjective() == null) 
            EnsureMinimalCameraDistance();

		// CameraFX
		ComputeFieldOfView(targetPosition);
		UpdateFieldOfView();

		Apply();
		ShakeUpdate();
		lastTargetPosition = targetPosition;
		rotationMultiplier = 1f;
	}

    public void SetTarget(Transform target)
    {
        this.target = target;
        UpdateVirtualTarget();
        virtualTarget.SetToDestination();
    }

    public Transform GetTarget()
    {
        return target;
    }

    public PositionAndRotation GetComputedPositionAndRotationDestination()
    {
        return new PositionAndRotation() { position = position.destination, rotation = rotationAroundTarget.destination };
    }

    public void UpdateVirtualTarget()
    {
        virtualTarget.destination = target.position;
        virtualTarget.current = new Vector3(
            target.position.x, 
            virtualTarget.current.y < target.position.y ? target.position.y : 
            Mathf.Lerp(virtualTarget.current.y, target.position.y, settings.virtualTargetYCatchUp * Time.fixedDeltaTime), 
            target.position.z
            );
    }

	public void ComputeHorizontalDistanceToTarget(Vector3 targetPosition)
    {
        hDistanceToTarget = Vector3.Distance(
            Vector3.Scale(new Vector3(1f, 0f, 1f), position.current),
            Vector3.Scale(new Vector3(1f, 0f, 1f), targetPosition)
        );
    }

    public float GetCatchUpSpeed()
    {
        float catchUpSpeed;
        catchUpSpeed = (hDistanceToTarget - settings.distance.min) / settings.distance.max;
        catchUpSpeed = Mathf.Clamp(Mathf.Abs(catchUpSpeed * settings.catchUpSpeedMultiplier), 0.4f, settings.maximumCatchUpSpeed);

        return catchUpSpeed;
    }

    public void ComputeRotation()
    {
        if (GetStaticObjective() != null) {
            rotationAroundTarget.destination = GetStaticObjective().positionAndRotation.rotation;
            return;
        }

        rotationAroundTarget.destination = Quaternion.Euler(0f, hAngle, vAngle);
    }

    public void ComputeFieldOfView(Vector3 targetPosition)
    {
        var targetMovementVelocity = Vector3.Distance(targetPosition, lastTargetPosition);
        var ratio = targetMovementVelocity * settings.speedEffectMultiplier;
        var fovMultiplier = 1 + ratio / 10f;
        fieldOfView.destination = fovMultiplier * settings.fieldOfView;
    }

    public void UpdateRotation()
    {
        float lerp = Time.fixedDeltaTime * (IsLookAtEnabled() ? settings.rotationSpeed * rotationMultiplier : settings.staticRotationLerp);
        var obj = GetStaticObjective();

        if (obj != null && obj.manualLerp.HasValue) {
            lerp = obj.manualLerp.Value;
        }

        rotationAroundTarget.current = Quaternion.Lerp(
            rotationAroundTarget.current,
            rotationAroundTarget.destination,
            lerp
        );
    }

    public void ComputePosition(Vector3 targetPosition)
    {
        if (GetStaticObjective() != null) {
            position.destination = GetStaticObjective().positionAndRotation.position;
            return;
        }

        // Dark pythagorian mathematics allow us to position the camera correctly
        Vector2 a = Vector3.Scale(new Vector3(0f, 1f, 1f), position.destination);
        Vector2 b = Vector3.Scale(new Vector3(0f, 1f, 1f), targetPosition);
        float t = settings.additionalAngle + settings.angleIncrementOnSpeed * Vector3.Distance(targetPosition, lastTargetPosition) + vAngle;
        t = Mathf.Clamp(t, -20f, 44f); // "Almost 45f". Don't put it to 45 or this algorithm will spit out NaNs
        float ab = Vector2.Distance(a, b);
        float bc = ab / Mathf.Cos(t * Mathf.Deg2Rad);
        float acSquare = Mathf.Pow(bc, 2f) - Mathf.Pow(ab, 2f);

        float cameraHeight = Mathf.Sqrt(Mathf.Abs(acSquare)); // (ac)

        var computedLocalPositionFromRot = new Vector3(Mathf.Sin(rotationAroundTarget.current.eulerAngles.y*Mathf.Deg2Rad), 0f, Mathf.Cos(rotationAroundTarget.current.eulerAngles.y * Mathf.Deg2Rad)) * settings.distance.min;

        position.destination =
            targetPosition
            + (cameraHeight + settings.heightOffset) * Vector3.up
            + computedLocalPositionFromRot;
            ;

        position.destination.y = Mathf.Min(position.destination.y, targetPosition.y + settings.maximumHeightAboveTarget);
    }

    public void UpdatePosition(float catchUpSpeed)
    {
        // Lerp on the up axis
        var verticalFollow = Time.fixedDeltaTime * settings.verticalFollowLerp * catchUpSpeed;
        var lateralFollow = Time.fixedDeltaTime * settings.lateralFollowLerp * catchUpSpeed;
        var longFollow = Time.fixedDeltaTime * settings.longitudinalFollowLerp * catchUpSpeed;

        if (GetStaticObjective() != null) {
            var statObj = GetStaticObjective();

            var lerp = statObj.manualLerp.HasValue ? statObj.manualLerp.Value : Time.fixedDeltaTime * settings.staticPositionLerp;

            if (statObj.manualLerp.HasValue) {
                position.current = Vector3.Lerp(position.current, position.destination, lerp);
                return;
            }
        }

        position.current.y = Mathf.Lerp(position.current.y, position.destination.y, verticalFollow);

        var rCurrent = cam.transform.InverseTransformPoint(position.current);
        var rDestination = cam.transform.InverseTransformPoint(position.destination);

        // Lerp on the right axis and forward axis
        rCurrent.x = Mathf.Lerp(rCurrent.x, rDestination.x, lateralFollow);
        rCurrent.z = Mathf.Lerp(rCurrent.z, rDestination.z, longFollow);

        position.current = cam.transform.TransformPoint(rCurrent);
    }

    public bool IsMovingToDestination()
    {
        return Vector3.Distance(position.current, position.destination) > destinationReachedThreshold || 
            Quaternion.Angle(rotationAroundTarget.current, rotationAroundTarget.destination) > rotationReachedThreshold;
    }

    public void UpdateFieldOfView()
    {
        fieldOfView.current = Mathf.Lerp(fieldOfView.current, fieldOfView.destination, Time.fixedDeltaTime * settings.fovLerp);
    }

    public void SetCurrentPositionAndRotation(PositionAndRotation pAR)
    {
        position.current = pAR.position;
        rotationAroundTarget.current = pAR.rotation;
    }

    public void EnsureMinimalCameraDistance()
    {
        if (target == null) return;

        // Absolute minimal distance so that whatever happens the camera can't be in my face
        var cameraDirection = -(Vector3.Scale(Vector3.one - Vector3.up, target.position) - Vector3.Scale(Vector3.one - Vector3.up, position.current));
        var dist = cameraDirection.magnitude;
        var detect = settings.detectsSurroundings && false; // Fix me/ enable me later

        if (detect) {
            RaycastHit hit;
            if (Physics.Raycast(target.position, cameraDirection, out hit)) {
                dist = Vector3.Distance(hit.point, target.position) - 0.1f;
            }
        }

        float outOfBounds = dist < settings.absoluteBoundaries.min && !detect ? settings.absoluteBoundaries.min : 
            dist > settings.absoluteBoundaries.max ? settings.absoluteBoundaries.max : 0f;

        if (outOfBounds != 0f) {
            position.current.x = target.position.x + cameraDirection.normalized.x * outOfBounds;
            position.current.z = target.position.z + cameraDirection.normalized.z * outOfBounds;
        }
    }

    public float GetHDistanceToTarget()
    {
        return hDistanceToTarget;
    }

    public void Shake(float intensity, float time)
    {
        shakeForce = intensity;
        shakeTimeRemaining = time;
    }
    
    void ShakeUpdate()
    {
        if (shakeTimeRemaining <= 0f) {
            shakeDisplacement = Vector3.zero;
            return;
        }

        lastUpdate = (lastUpdate + 1) % updateEvery;
        if (lastUpdate != 1) {
            return;
        }

        shakeTimeRemaining -= Time.deltaTime;
        shakeDisplacement = ((UnityEngine.Random.insideUnitSphere * 2f) - Vector3.one) * shakeForce;
    }

    public void Apply()
    {
        // Look at 
        if (IsLookAtEnabled()) {
            // The further the player is from the center of the screen, the quickest i should be to look at the player
            var screenPosition = Vector3.Scale(Vector3.up + Vector3.right, cam.WorldToViewportPoint(virtualTarget.current) - Vector3.up / 2f - Vector3.right / 2f);
            var screenPosition2 = new Vector2(screenPosition.x, screenPosition.y);

            cam.transform.forward = Vector3.Lerp(
                cam.transform.forward,
                -(position.current - (virtualTarget.current + settings.heightOffset * Vector3.up)).normalized,
                settings.lookAtLerp * Time.fixedDeltaTime 
                    // Off-screen bonus to look at the target
                    + Mathf.Abs(1f / (1f - screenPosition2.magnitude) - 1f)
            );
        }
        else {
            cam.transform.rotation = rotationAroundTarget.current;
        }
        
        cam.transform.position = position.current + (GetStaticObjective() == null ? settings.heightOffset * Vector3.up : Vector3.zero) + shakeDisplacement;
        cam.fieldOfView = fieldOfView.current;

    } // Apply the values to the camera 

    public void Focus(Vector3 newPosition, AperturePreset set = null)
    {
        defaultTarget = newPosition;
        target = null;
        if(set != null) Load(set);
    } // Focus camera on a new position (Vector3)

    public void Focus(Transform newTarget, AperturePreset set = null)
    {
        target = newTarget;
        if(set != null) Load(set);
    } // Focus camera on a new target (transform)

    public void Teleport()
    {
        position.SetToDestination();
        rotationAroundTarget.SetToDestination();
        fieldOfView.SetToDestination();
        if (target) virtualTarget.SetToDestination();
        stackTransitionState = 1f;

        Apply();
    } // Teleport all the camera values instantly (to ignore lerp)

    public void UpdateAndTeleport()
    {
        FixedUpdate();
        Teleport();
    }
    
    public PositionAndRotation AddStaticPosition(Transform transform)
    {
        return AddStaticPosition(new PositionAndRotation() { position = transform.position, rotation = transform.rotation });
    }

    public PositionAndRotation AddStaticPosition(Vector3 position, Quaternion rotation)
    {
        return AddStaticPosition(new PositionAndRotation() { position = position, rotation = rotation });
    }

    public PositionAndRotation AddStaticPosition(PositionAndRotation positionAndRotation)
    {
        return AddStaticObjective(new StaticObjective(positionAndRotation)).positionAndRotation;
    }

    public StaticObjective AddStaticObjective(StaticObjective obj)
    {
        staticObjectives.Add(obj);
        return obj;
    }

    public void RemoveStaticPosition(PositionAndRotation positionAndRotation)
    {
        staticObjectives.RemoveAll(o => o.positionAndRotation == positionAndRotation);
    }

    public void RemoveStaticObjective(StaticObjective positionAndRotation)
    {
        staticObjectives.RemoveAll(o => object.ReferenceEquals(o, positionAndRotation));
    }

    public StaticObjective GetStaticObjective()
    {
        return staticObjectives.Count > 0 ? staticObjectives[staticObjectives.Count - 1] : null;
    }

    public float GetLastCameraInput()
    {
        return lastCameraInput;
    }

    public Vector3 GetVirtualTarget()
    {
        return Vector3.Scale(virtualTarget.current, Vector3.one);
    }

    public int GetStaticPositionsCount()
    {
        return staticObjectives.Count;
    }

    public int GetStackSize()
    {
        return presetStack.Count;
    }

    public string GetStackNames()
    {
        return string.Join(", ", presetStack.Select(o => { return o.name; }));
    }

    public Transform GetCameraTransform()
    {
        return cam.transform;
    }

    public float GetHAngle()
    {
        return hAngle;
    }

    public float GetVAngle()
    {
        return vAngle;
    }

    public static Camera GetCurrentlyActiveCamera()
    {
        var activeCams = Camera.allCameras.Where(o => { return o.isActiveAndEnabled; });
        return activeCams.Count() > 0 ? activeCams.First() : null;
    }
}