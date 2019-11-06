using System;
using UnityEditor;
using UnityEngine;

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

    // SPEED
    Vector3 lastTargetPosition;
    float hDistanceToTarget = 0f;

    // SHAKE
    float timer = 0f;
    float intensity = 0.7f;
    float duration = 0f;

    // Current angle on Y axis
    float hAngle;
    // Current angle on X axis
    float vAngle;
    float vAngleAmount;
    bool isCameraBeingRepositioned;
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
        cam = Camera.main;
        Load(Game.i.library.defaultAperture);
    }

    public Aperture(AperturePreset s)
    {
        cam = Camera.main;
        Load(s);
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
        hAngle += rot.y * settings.cameraRotateAroundSensivity * Time.deltaTime;
        vAngleAmount = Mathf.Lerp(vAngleAmount, Mathf.Lerp(vAngleAmount, rot.x, settings.cameraRotateAboveSensivity * Time.deltaTime), Mathf.Abs(rot.x));

        if (rot.magnitude > 0f) {
            lastCameraInput = Time.time;
            isCameraBeingRepositioned = false;
        }
    }
    public void Rotate(float x = 0f, float y = 0f, float z = 0f)
    {
        Rotate(new Vector3(x, y, z));
    }

    public void Update()
    {
        if (Time.time - lastCameraInput > settings.cameraResetAfterTime) {
            isCameraBeingRepositioned = true;
        }
    }

    public void FixedUpdate()
    {
        if (freeze) return;
        if (settings.constraintToTarget) {
            cam.transform.parent = target;
            cam.transform.localPosition = settings.targetConstraintLocalOffset;
            cam.transform.forward = target.forward;
            return;
        }
        else cam.transform.parent = null;
        
        var targetPosition = target ? target.position : defaultTarget;

        // Camera trying to get in my back
        var targetMovementVelocity = Vector3.Distance(targetPosition, lastTargetPosition);
        if (targetMovementVelocity > 0 || isCameraBeingRepositioned) {
            hAngle = Mathf.Lerp(hAngle, 0f, Time.fixedDeltaTime * settings.cameraResetSpeed);
            vAngleAmount = Mathf.Lerp(vAngleAmount, 0f, Time.fixedDeltaTime * settings.cameraResetSpeed);
        }
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
        EnsureMinimalCameraDistance();

        // CameraFX
        ComputeFieldOfView(targetPosition);
        UpdateFieldOfView();
        
        Apply();
        ShakeUpdate();

        lastTargetPosition = targetPosition;
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
        var bestHAngle = Vector3.SignedAngle(Vector3.forward, target.forward, Vector3.up);
        var angle = hAngle + bestHAngle;
        rotationAroundTarget.destination = -new Vector3(Mathf.Sin(Mathf.Deg2Rad * angle), 0f, Mathf.Cos(Mathf.Deg2Rad * angle));
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
        rotationAroundTarget.current = Vector3.Lerp(rotationAroundTarget.current, rotationAroundTarget.destination, Time.fixedDeltaTime * settings.rotationSpeed);
    }

    public void ComputePosition(Vector3 targetPosition)
    {
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
        cam.transform.forward = Vector3.Lerp(
            cam.transform.forward, 
            -(position.current - (target.position + settings.heightOffset * Vector3.up)).normalized, 
            settings.lookAtLerp * Time.fixedDeltaTime
        );

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
}