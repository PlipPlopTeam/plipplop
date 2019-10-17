using System;
using UnityEditor;
using UnityEngine;

public class Aperture
{
    [Serializable]
    public class Settings
    {
        [Header("Basics")]
        public bool canBeControlled = true;

        [Range(2f, 200f)] public float fieldOfView = 75f;
        public float heightOffset;
        [Range(0f, 40f)] public float additionalAngle = 20f;
        public Range distance;

        [Header("Lerps")]
        public float fovLerp = 1f;
        public float followLerp = 1f;
        //TODO: Reimplement
        //public float camLerp = 1f;
        public float rotationSpeed = 1f;

        [Header("Speed Enhancer")]
        [Range(0.1f, 10)]  public float speedEffectMultiplier = 1f;
        [Range(1f, 20f)] public float catchUpSpeedMultiplier = 1f;

        [Header("Advanced")]
        public float maximumCatchUpSpeed = 10f;
        public float cameraRotateAroundSpeed = 4f;
    }

    public class Key<T>
    {
        public T origin;
        public T destination;
        public T current;

        public Key() { }
        public Key(T value)
        {
            origin = value;
            destination = value;
            current = value;
        }

        public void Reset()
        {
            current = origin;
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

    public Key<float> fieldOfView;
    public Key<Vector3> position;
    public Key<Vector3> rotationAroundTarget;

    Settings settings;

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

    void Load()
    {
        // POSITION
        position = new Key<Vector3>();

        // ROTATION
        rotationAroundTarget = new Key<Vector3>();

        // FOV
        fieldOfView = new Key<float>(settings.fieldOfView);
    }

    public void Load(Settings s)
    {
        settings = s;
        Load();
    }

    public Settings GetSettings()
    {
        return settings;
    }

    public Aperture()
    {
        cam = Camera.main;
        Load(Game.i.library.defaultAperture.settings);
    }

    public Aperture(Settings s)
    {
        cam = Camera.main;
        Load(s);
    }

    public Vector3 Forward()
    {
        return Vector3.ClampMagnitude(
            new Vector3(cam.transform.forward.x, 0f, cam.transform.forward.z),
            1f
        );
    }

    public Vector3 Right()
    {
        return cam.transform.right;
    }

    public void RotateWithGamepad(float x = 0f, float y = 0f, float z = 0f)
    {
        if (settings.canBeControlled) Rotate(x, y, z);
    }

    public void Rotate(Vector3 rot)
    {
        hAngle += rot.y * settings.cameraRotateAroundSpeed;
    }
    public void Rotate(float x = 0f, float y = 0f, float z = 0f)
    {
        hAngle += y * settings.cameraRotateAroundSpeed;
    }

    public void Update()
    {

    }

    public void FixedUpdate()
    {

        float catchUpSpeed = 1f;
        float fovMultiplier = 0f;
        float targetMovementVelocity;

        hAngle = Mathf.Lerp(hAngle, 0f, Time.fixedDeltaTime * 3f);

        if (target != null) 
        {
            // Distance based on X and Z axises only
            // Distance between camera and target
            hDistanceToTarget = Vector3.Distance(
                Vector3.Scale(new Vector3(1f, 0f, 1f), position.current), 
                Vector3.Scale(new Vector3(1f, 0f, 1f), target.position)
            );


            var bestHAngle = Vector3.SignedAngle(Vector3.forward, target.forward, Vector3.up);
            //hAngle = bestHAngle;
            var angle = hAngle + bestHAngle;
            rotationAroundTarget.destination = -new Vector3(Mathf.Sin(Mathf.Deg2Rad * angle), 0f, Mathf.Cos(Mathf.Deg2Rad * angle));

            targetMovementVelocity = Vector3.Distance(target.position, lastTargetPosition);

            // The Speed Enhancement effect
            float ratio = targetMovementVelocity * settings.speedEffectMultiplier;
            fovMultiplier = 1 + ratio/10f;

            // The further the camera is, the fastest we want to catch up
            catchUpSpeed = (hDistanceToTarget - settings.distance.min) / settings.distance.max;
            catchUpSpeed = Mathf.Clamp(Mathf.Abs(catchUpSpeed * settings.catchUpSpeedMultiplier), 0.4f, settings.maximumCatchUpSpeed);

            lastTargetPosition = target.position;
        }

        /*
        Vector3 horizontalDirection = new Vector3(targetMovementDirection.x, 0f, targetMovementDirection.z);
        var hAngleDifference = Vector3.SignedAngle(Forward(), horizontalDirection, Vector3.up);


        // TODO : REimplement angle limits

        if(hAngleDifference >= 0) hAngleDifference -= 180f;
        else hAngleDifference += 180f;

        if(hAngleDifference > maxAngle || hAngleDifference < -maxAngle) hAngleDifference = 0f;
        Rotate(0f, (-hAngleDifference / maxAngle) * settings.rotSpeed, 0f);
        
        if (rotationAroundTarget.current.x + settings.angle > -settings.rotationClamp.min)
            rotationAroundTarget.current.x = -settings.rotationClamp.min + settings.angle;
        else if(rotationAroundTarget.current.x + settings.angle < -settings.rotationClamp.max)
            rotationAroundTarget.current.x = -settings.rotationClamp.max + settings.angle;
        */


        // Dark pythagorian mathematics allow us to position the camera correctly
        Vector2 a = Vector3.Scale(new Vector3(0f, 1f, 1f), position.destination);
        Vector2 b = Vector3.Scale(new Vector3(0f, 1f, 1f), target.position);
        float t = settings.additionalAngle;
        float ab = Vector2.Distance(a, b);
        float bc = ab / Mathf.Cos(t*Mathf.Deg2Rad);
        float acSquare = Mathf.Pow(bc, 2f) - Mathf.Pow(ab, 2f);

        float cameraHeight = Mathf.Sqrt(Mathf.Abs(acSquare));

        rotationAroundTarget.current = Vector3.Lerp(rotationAroundTarget.current, rotationAroundTarget.destination, Time.fixedDeltaTime * settings.rotationSpeed);

        position.destination = 
            target.position
            + (cameraHeight + settings.heightOffset) * Vector3.up
            + rotationAroundTarget.current * Mathf.Clamp(hDistanceToTarget, settings.distance.min, settings.distance.max)
            ;
        
        position.current = Vector3.Lerp(
            position.current, 
            position.destination, 
            Time.deltaTime * settings.followLerp * catchUpSpeed
        );

        fieldOfView.destination = fovMultiplier * settings.fieldOfView;
        fieldOfView.current = Mathf.Lerp(fieldOfView.current, fieldOfView.destination, Time.fixedDeltaTime * settings.fovLerp);
        
        Apply();
        ShakeUpdate();
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
        cam.transform.forward = -(position.current - (target.position + settings.heightOffset * Vector3.up)).normalized;

        cam.transform.position = position.current;
        cam.fieldOfView = fieldOfView.current;
    } // Apply the values to the camera 

    public void Focus(Vector3 newPosition, Settings set = null)
    {
        defaultTarget = newPosition;
        target = null;
        if(set != null) Load(set);
    } // Focus camera on a new position (Vector3)

    public void Focus(Transform newTarget, Settings set = null)
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

    public void Reset()
    {
        target = null;
        position.Reset();
        rotationAroundTarget.Reset();
        fieldOfView.Reset();

    } // Reset all the values to the origin values

    public void SwitchCamera(Camera newCam)
    {
        cam.enabled = false;
        newCam.enabled = true;
    }
}