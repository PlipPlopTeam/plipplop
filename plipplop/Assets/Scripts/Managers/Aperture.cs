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
        public Range distance;
        public Range rotationClamp;

        [Header("Lerps")]
        public float fovLerp = 1f;
        public float followLerp = 1f;
        //TODO: Reimplement
        //public float camLerp = 1f;
        //public float rotSpeed = 1f;

        [Header("Speed Enhancer")]
        [Range(0.1f, 10)]  public float speedEffectMultiplier = 1f;
        [Range(1f, 20f)] public float catchUpSpeedMultiplier = 1f;

        [Header("Advanced")]
        public float maximumCatchUpSpeed = 10f;
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

    // Look right look left
    float leftRightAccumulator;

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
        rotationAroundTarget.destination += rot;
    }
    public void Rotate(float x = 0f, float y = 0f, float z = 0f)
    {
        rotationAroundTarget.destination += new Vector3(x, y, z);
    }

    public void Update()
    {

    }

    public void FixedUpdate()
    {

        Vector3 hDirectionToTarget = Vector3.zero;
        float catchUpSpeed = 1f;
        float fovMultiplier = 0f;
        float targetMovementVelocity = 0f;

        if (target != null) 
        {
            // Distance based on X and Z axises only
            // Distance between camera and target
            hDistanceToTarget = Vector3.Distance(
                Vector3.Scale(new Vector3(1f, 0f, 1f), position.current), 
                Vector3.Scale(new Vector3(1f, 0f, 1f), target.position)
            );

            // Direction only on the X Z axis
            hDirectionToTarget = (
                Vector3.Scale(new Vector3(1f, 0f, 1f), position.current) - 
                Vector3.Scale(new Vector3(1f, 0f, 1f), target.position)
            ).normalized;


            targetMovementVelocity = Vector3.Distance(target.position, lastTargetPosition);

            // The Speed Enhancement effect
            float ratio = targetMovementVelocity * settings.speedEffectMultiplier;
            fovMultiplier = 1 + ratio/10f;

            // The further the camera is, the fastest we want to catch up
            catchUpSpeed = (hDistanceToTarget - settings.distance.min) / settings.distance.max;
            catchUpSpeed = Mathf.Clamp(Mathf.Abs(catchUpSpeed * settings.catchUpSpeedMultiplier), 0.4f, settings.maximumCatchUpSpeed);

            lastTargetPosition = target.position;
        }

        // TODO : REimplement angle limits
        /*
        Vector3 horizontalDirection = new Vector3(targetMovementDirection.x, 0f, targetMovementDirection.z);
        hAngleDifference = Vector3.SignedAngle(Forward(), horizontalDirection, Vector3.up);

        if(hAngleDifference >= 0) hAngleDifference -= 180f;
        else hAngleDifference += 180f;

        if(hAngleDifference > maxAngle || hAngleDifference < -maxAngle) hAngleDifference = 0f;
        Rotate(0f, (-hAngleDifference / maxAngle) * settings.rotSpeed, 0f);


        if(rotationAroundTarget.current.x + settings.angle > -settings.rotationClamp.min)
            rotationAroundTarget.current.x = -settings.rotationClamp.min + settings.angle;
        else if(rotationAroundTarget.current.x + settings.angle < -settings.rotationClamp.max)
            rotationAroundTarget.current.x = -settings.rotationClamp.max + settings.angle;
        */

        position.destination = 
            target.position
            + settings.heightOffset * Vector3.up
            + hDirectionToTarget * Mathf.Clamp(hDistanceToTarget, settings.distance.min, settings.distance.max);

        position.current = Vector3.Lerp(
            position.current, 
            position.destination, 
            Time.deltaTime * settings.followLerp * catchUpSpeed
        );
        
        fieldOfView.destination = fovMultiplier * settings.fieldOfView;
        fieldOfView.current = Mathf.Lerp(fieldOfView.current, fieldOfView.destination, Time.deltaTime * settings.fovLerp);
        
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