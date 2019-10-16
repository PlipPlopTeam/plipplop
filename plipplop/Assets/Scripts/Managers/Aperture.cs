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

    class Key<T>
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

    [Header("References")]
    public Camera cam;
    public Transform target;

    [Header("Settings")]
    Settings settings;

    [Header("Speed Enhancer")]
    public float maxEffect = 2f;
    public float multiplier = 1f;
    public Vector2 distanceRange;
    public Vector2 fovRange;

    // MOVEMENT
    Key<float> fieldOfView;
    Key<float> distance;
    
    Vector3 defaultTarget;

    Key<Vector3> position;
    Key<Vector3> rotation;

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

    public Aperture()
    {
        cam = Camera.main;
        Load(Game.i.library.defaultAperture.settings);
    }

#region INSPECTOR 
    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color32(173, 216, 230, 255);
        UnityEditor.Handles.color = Gizmos.color = new Color32(173, 216, 230, 255);
        GUIStyle style = new GUIStyle();
        style.normal.textColor = new Color32(173, 216, 230, 255);
        style.alignment = TextAnchor.MiddleCenter;
        style.fontStyle = FontStyle.Bold;
        style.fontSize = 9;

        if(target)
        {
            //Debug.Log(settings);
            // Follow range draw
            UnityEditor.Handles.DrawWireDisc(position.current, Vector3.up, settings.range.x);
            UnityEditor.Handles.DrawWireDisc(position.current, Vector3.up, settings.range.y);
            Gizmos.DrawLine(cam.transform.position, position.current);
            Gizmos.DrawLine(position.current, target.position);
            UnityEditor.Handles.Label(position.current + Vector3.right * settings.range.x, "Min " + settings.range.x.ToString(), style);
            UnityEditor.Handles.Label(position.current + Vector3.right * settings.range.y, "Max " + settings.range.y.ToString(), style);
            UnityEditor.Handles.Label((cam.transform.position + target.position)/2, "Dist " + distance.current.ToString(), style);

            Gizmos.DrawLine(cam.transform.position, position.target);
        }
        else
        {
            Gizmos.DrawLine(cam.transform.position, defaultTarget);
        }

        //Gizmos.DrawLine(transform.position,
        //    transform.position + new Vector3( transform.forward.x, 0f, transform.forward.z)
        //);
    }
    
    // Execute when editing values in the inspector
    void OnDrawGizmos()
    {
        if (!EditorApplication.isPlaying) {
            settings = Game.i.library.defaultAperture.settings;
            var controller = target.GetComponent<Controller>();
            if (controller && controller.customCamera) {
                settings = controller.customCamera.settings;
            }

            Load(settings);

            if (settings.distance < 0f) settings.distance = 0f;
            if (settings.fieldOfView < 2f) settings.fieldOfView = 2f;
            if (distance.target < 0) distance.target = 0f;
            if (settings.range.y < settings.range.x) settings.range.y = settings.range.x;
            if (target != null) position.current += target.position;

            position.target = position.current + Quaternion.Euler(rotation.current) * Vector3.forward * distance.current;
            cam.transform.position = position.target;
        }
    }

    #endregion

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
            float ratio = Mathf.Clamp(targetMovementVelocity / maxEffect, 0f, 1f);
            fovOffset = (fovRange.x + (fovRange.y - fovRange.x) * ratio) * multiplier;
            distanceOffset = (distanceRange.x + (distanceRange.y - distanceRange.x) * ratio) * multiplier;

            lastTargetPosition = target.position;
        }

        Vector3 horizontalDirection = new Vector3(targetMovementDirection.x, 0f, targetMovementDirection.z);
        if(horizontalDirection.magnitude > 0.1f)
        {
            angleDifference = Vector3.SignedAngle(Forward(), horizontalDirection, Vector3.up);
            if(angleDifference >= 0) angleDifference -= 180f;
            else angleDifference += 180f;
            if(angleDifference > maxAngle || angleDifference < -maxAngle) angleDifference = 0f;
            Rotate(0f, (-angleDifference/maxAngle) * settings.rotSpeed, 0f);
        }

        if(distanceFromTarget > settings.range.x)
        {
            position.current = Vector3.Lerp(position.current, defaultTarget + offset, Time.deltaTime * settings.followLerp * speed); 
        }

        fieldOfView.current = Mathf.Lerp(fieldOfView.current, fieldOfView.target + fovOffset, Time.deltaTime * settings.fovLerp);
        distance.current = distance.target + distanceOffset;

        Vector3 rotOffset = new Vector3(settings.angle, 0f, 0f);

        if(rotation.current.x + rotOffset.x > -settings.rotationClamp.x)
            rotation.current.x = -settings.rotationClamp.x + rotOffset.x;
        else if(rotation.current.x + rotOffset.x < -settings.rotationClamp.y)
            rotation.current.x = -settings.rotationClamp.y + rotOffset.x;

        // Applying current values 
        position.target = position.current + Quaternion.Euler(rotation.current + rotOffset) * Vector3.forward * distance.current;
        cam.transform.forward = -(cam.transform.position - position.current).normalized;

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
            rotation.current += Random.insideUnitSphere * intensity;
            intensity *= timer/duration;
            if(timer <= 0) Teleport();
        }
    }

    void Apply()
    {
        cam.transform.forward = -(cam.transform.position - position.current).normalized;
        cam.transform.position = Vector3.Lerp(cam.transform.position, position.target, Time.deltaTime * settings.camLerp);
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