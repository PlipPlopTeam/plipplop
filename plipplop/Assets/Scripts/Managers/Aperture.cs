using System;
using UnityEditor;
using UnityEngine;

public class Aperture
{
    [System.Serializable]
    public class Settings
    {
        [Header("Basics")]
        public bool canBeControlled = true;
        public float distance = 1f;
        [Range(2f, 200f)] public float fieldOfView = 75f;
        public float angle;
        public Vector3 positionOffset;
        public Vector2 range;
        public Vector2 rotationClamp;
        [Header("Lerps")]
        public float fovLerp = 1f;
        public float followLerp = 1f;
        public float camLerp = 1f;
        public float rotSpeed = 1f;
        [Header("Speed Enhancer")]
        public float speedEffectMultiplier = 1f;
    }

    public class Key<T>
    {
        public T origin;
        public T target;
        public T current;

        public Key() { }
        public Key(T value)
        {
            origin = value;
            target = value;
            current = value;
        }

        public void Reset()
        {
            current = origin;
        }

        public void SetToTarget()
        {
            current = target;
        }
    }

    public Camera cam;
    public Transform target;

    public Key<float> fieldOfView;
    public Key<float> distance;
    public Key<Vector3> position;
    public Key<Vector3> rotation;

    Settings settings;

    float maxEffect = 2f;
    float multiplier = 1f;
    Vector2 distanceRange;
    Vector2 fovRange;
    
    Vector3 defaultTarget;

    // SPEED
    float distanceOffset;
    float fovOffset;
    Vector3 lastTargetPosition;

    // SHAKE
    float timer = 0f;
    float intensity = 0.7f;
    float duration = 0f;

    void Load()
    {
        // POSITION
        position = new Key<Vector3>(settings.positionOffset);

        // ROTATION
        rotation = new Key<Vector3>() { current = new Vector3(settings.angle, 0f, 0f) };

        // FOV
        fieldOfView = new Key<float>(settings.fieldOfView);

        // DISTANCE
        distance = new Key<float>(settings.distance);
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
        rotation.current += rot;
    }
    public void Rotate(float x = 0f, float y = 0f, float z = 0f)
    {
        rotation.current += new Vector3(x, y, z);
    }

    public void Update()
    {

    }

    public void FixedUpdate()
    {
        // Distance cannot be less than 0
        if (settings.distance < 0) settings.distance = 0;

        // Initializing values
        Vector3 offset = Vector3.zero;
        Vector3 targetMovementDirection = Vector3.zero;
        float distanceFromTarget = 0f;
        float targetMovementVelocity = 0f;
        float speed = 1f;
        float angleDifference = 0f;
        float maxAngle = 100f;

        // If Camera if following a target
        if(target != null) 
        {
            offset = target.position;

            // Getting usefull values about target
            distanceFromTarget = Vector3.Distance(position.current, target.position);
            targetMovementVelocity = Vector3.Distance(target.position, lastTargetPosition);
            targetMovementDirection = (target.position - lastTargetPosition).normalized;

            // Increasing the position lerp if the target go further the distanceRange
            speed = (distanceFromTarget - settings.range.x) / settings.range.y;

            // The Speed Enhancement effect
            /*
            float ratio = Mathf.Clamp(targetMovementVelocity / maxEffect, 0f, 1f);
            fovOffset = (fovRange.x + (fovRange.y - fovRange.x) * ratio) * multiplier;
            distanceOffset = (distanceRange.x + (distanceRange.y - distanceRange.x) * ratio) * multiplier;
            */
            lastTargetPosition = target.position;
        }

        Vector3 horizontalDirection = new Vector3(targetMovementDirection.x, 0f, targetMovementDirection.z);

        angleDifference = Vector3.SignedAngle(Forward(), horizontalDirection, Vector3.up);

        if(angleDifference >= 0) angleDifference -= 180f;
        else angleDifference += 180f;

        if(angleDifference > maxAngle || angleDifference < -maxAngle) angleDifference = 0f;
        Rotate(0f, (-angleDifference/maxAngle) * settings.rotSpeed, 0f);

        fieldOfView.current = Mathf.Lerp(fieldOfView.current, fieldOfView.target + fovOffset, Time.deltaTime * settings.fovLerp);
        distance.current = distance.target + distanceOffset;

        Vector3 rotOffset = new Vector3(settings.angle, 0f, 0f);

        if(rotation.current.x + rotOffset.x > -settings.rotationClamp.x)
            rotation.current.x = -settings.rotationClamp.x + rotOffset.x;
        else if(rotation.current.x + rotOffset.x < -settings.rotationClamp.y)
            rotation.current.x = -settings.rotationClamp.y + rotOffset.x;

        // Applying current values 
        if (distanceFromTarget > settings.range.x) {
            position.target = offset + Quaternion.Euler(rotation.current + rotOffset) * Vector3.forward * distance.current;
        }

        position.current = Vector3.Lerp(
            position.current, 
            position.target, 
            Time.deltaTime * settings.followLerp * speed
        );


        Apply();
        ShakeUpdate();
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
            rotation.current += UnityEngine.Random.insideUnitSphere * intensity;
            intensity *= timer/duration;
            if(timer <= 0) Teleport();
        }
    }

    public void Apply()
    {
        cam.transform.forward = -(position.current - target.position).normalized;
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
        position.SetToTarget();
        rotation.SetToTarget();
        fieldOfView.SetToTarget();
        Apply();
    } // Teleport all the camera values instantly (to ignore lerp)

    public void Reset()
    {
        target = null;
        position.Reset();
        rotation.Reset();
        fieldOfView.Reset();
        distance.Reset();
    } // Reset all the values to the origin values

    public void SwitchCamera(Camera newCam)
    {
        cam.enabled = false;
        newCam.enabled = true;
    }
}