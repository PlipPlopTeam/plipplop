using System.Collections.Generic;
using System;
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

    [Serializable]
    public class Range
    {
        public float min;
        public float max;
    }

    public Camera cam;
    public Transform target;

    public Key<float> fieldOfView = new Key<float>();
    public Key<Vector3> position = new Key<Vector3>();
    public Key<Vector3> rotationAroundTarget = new Key<Vector3>();

    AperturePreset settings;
    Vector3 defaultTarget;

    // Static cameras
    List<PositionAndRotation> staticObjectives = new List<PositionAndRotation>();
    List<bool> lookAtTarget = new List<bool>();

    // SPEED
    Vector3 lastTargetPosition;
    float hDistanceToTarget = 0f;

    // SHAKE
    float timer = 0f;
    float intensity = 0.7f;
    float duration = 0f;

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
    float lastCameraInput;

    bool freeze = false;
    public void Freeze() {freeze = true;}
    public void Unfreeze() {freeze = false;}

    public void Load(AperturePreset s)
    {
        settings = s;
    }

    public AperturePreset GetSettings()
    {
        return settings;
    }

    public Aperture()
    {
        cam = Camera.main ?? new GameObject().AddComponent<Camera>();
        cam.gameObject.name = "_CAMERA";
        Load(Game.i.library.defaultAperture);
    }

    public Aperture(AperturePreset s)
    {
        cam = Camera.main;
        Load(s);
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
			Realign();
		}
	}

    public void Realign()
    {
		rotationMultiplier = settings.alignMultiplierByUser;
		isCameraBeingRepositioned = true;
    }
	public void Aligned()
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

	public void FixedUpdate()
	{
		if (freeze) return;
		if (settings.constraintToTarget)
		{
			cam.transform.parent = target;
			cam.transform.localPosition = settings.targetConstraintLocalOffset;
			cam.transform.forward = target.forward;
			return;
		}
		else cam.transform.parent = null;

		var targetPosition = target ? target.position : defaultTarget;

		// Camera trying to get in my back
		var targetMovementVelocity = Vector3.Distance(targetPosition, lastTargetPosition);
		if (!isTargetMoving)
		{
			if (targetMovementVelocity > settings.minTargetVelocity)
			{
				isTargetMoving = true;
				// START MOVING
			}
		}
		else
		{
			if (targetMovementVelocity <= settings.minTargetVelocity)
			{
				hAngle = Vector3.SignedAngle(Vector3.forward, Forward(), Vector3.up);
				isTargetMoving = false;
				// STOP MOVING
			}
		}

		// ALIGNING
		if (isCameraBeingRepositioned)
		{
			hAngle = Vector3.SignedAngle(Vector3.forward, target.forward, Vector3.up);
			float a = Vector3.SignedAngle(Forward(), target.forward, Vector3.up);
			if (Mathf.Abs(a) < settings.angleConsideredAlign) Aligned();
		}
		// TARGET IS MOVING
		else if (isTargetMoving && Mathf.Abs(hAngleAmount) <= 0f)
		{
			ResetIdleTime();
			float sForwardAngle = Vector3.SignedAngle(Vector3.forward, target.forward, Vector3.up);
			float diffForwardAngle = Mathf.Abs(Vector3.SignedAngle(Forward(), target.forward, Vector3.up));
			float angDifMultiplier = 1f;
			if (diffForwardAngle >= 90f || diffForwardAngle <= -90f)
			{
				angDifMultiplier = 1f - Mathf.Abs(1f - (diffForwardAngle / 90f));
			}
			rotationMultiplier = targetMovementVelocity * settings.alignMultiplierByVelocity * angDifMultiplier;
			hAngle = sForwardAngle;
			hAngle += hAngleAmount * settings.alignMultiplierByStick;
		}
		else
		{
			hAngle += hAngleAmount;
		}
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
            rotationAroundTarget.destination = GetStaticObjective().euler;
            return;
        }
        rotationAroundTarget.destination = -new Vector3(Mathf.Sin(Mathf.Deg2Rad * hAngle), 0f, Mathf.Cos(Mathf.Deg2Rad * hAngle));
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
        rotationAroundTarget.current = Vector3.Lerp(rotationAroundTarget.current, rotationAroundTarget.destination, Time.fixedDeltaTime * settings.rotationSpeed * rotationMultiplier);
    }

    public void ComputePosition(Vector3 targetPosition)
    {
        if (GetStaticObjective() != null) {
            position.destination = GetStaticObjective().position;
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

        position.destination =
            targetPosition
            + (cameraHeight + settings.heightOffset) * Vector3.up
            + rotationAroundTarget.current * Mathf.Clamp(hDistanceToTarget, settings.distance.min, settings.distance.max)
            ;
    }

    public void UpdatePosition(float catchUpSpeed)
    {
        // Lerp on the up axis
        var verticalFollow = Time.fixedDeltaTime * settings.verticalFollowLerp * catchUpSpeed;
        position.current.y = Mathf.Lerp(position.current.y, position.destination.y, verticalFollow);

        var lateralFollow = Time.fixedDeltaTime * settings.lateralFollowLerp * catchUpSpeed;
        var longFollow = Time.fixedDeltaTime * settings.longitudinalFollowLerp * catchUpSpeed;

        var rCurrent = cam.transform.InverseTransformPoint(position.current);
        var rDestination = cam.transform.InverseTransformPoint(position.destination);

        // Lerp on the right axis and forward axis
        rCurrent.x = Mathf.Lerp(rCurrent.x, rDestination.x, lateralFollow);
        rCurrent.z = Mathf.Lerp(rCurrent.z, rDestination.z, longFollow);

        position.current = cam.transform.TransformPoint(rCurrent);
    }

    public void UpdateFieldOfView()
    {
        fieldOfView.current = Mathf.Lerp(fieldOfView.current, fieldOfView.destination, Time.fixedDeltaTime * settings.fovLerp);
    }

    public void EnsureMinimalCameraDistance()
    {
        // Absolute minimal distance so that whatever happens the camera can't be in my face
        var cameraDirection = -(Vector3.Scale(Vector3.one - Vector3.up, target.position) - Vector3.Scale(Vector3.one - Vector3.up, position.current));
        float outOfBounds = cameraDirection.magnitude < settings.absoluteBoundaries.min ? settings.absoluteBoundaries.min : cameraDirection.magnitude > settings.absoluteBoundaries.max ? settings.absoluteBoundaries.max : 0f;
        if (outOfBounds != 0f) {
            position.current.x = target.position.x + cameraDirection.normalized.x * outOfBounds;
            position.current.z = target.position.z + cameraDirection.normalized.z * outOfBounds;
        }
    }

    public float GetHDistanceToTarget()
    {
        return hDistanceToTarget;
    }

    [ContextMenu("Shake")]
    public void DEBUG_Shake() {Shake(5f, 2f);}
    public void Shake(float i = 0.5f, float d = 1f)
    {   
        intensity = i;
        duration = d;
        timer = duration;
    }
    public void ShakeUpdate()
    {
        if(timer > 0)
        {
            timer -= Time.deltaTime;
            // TODO: Update
            rotationAroundTarget.current += UnityEngine.Random.insideUnitSphere * intensity;
            intensity *= timer/duration;
            if(timer <= 0) Teleport();
        }
    }

    public void Apply()
    {
        // Look at 
        if (IsLookAtEnabled()) {
            cam.transform.forward = Vector3.Lerp(
                cam.transform.forward,
                -(position.current - (target.position + settings.heightOffset * Vector3.up)).normalized,
                settings.lookAtLerp * Time.fixedDeltaTime
            );
        }

        cam.transform.position = position.current;
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
        Apply();
    } // Teleport all the camera values instantly (to ignore lerp)

    public void SwitchCamera(Camera newCam)
    {
        cam.enabled = false;
        newCam.enabled = true;
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
        staticObjectives.Add(positionAndRotation);
        return positionAndRotation;

    }

    public void RemoveStaticPosition(PositionAndRotation positionAndRotation)
    {
        staticObjectives.RemoveAll(o => o.Equals(positionAndRotation));
    }

    public PositionAndRotation GetStaticObjective()
    {
        return staticObjectives.Count > 0 ? staticObjectives[staticObjectives.Count - 1] : null;
    }
}