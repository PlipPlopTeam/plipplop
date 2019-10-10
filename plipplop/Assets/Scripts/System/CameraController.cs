using UnityEngine;

[System.Serializable]
public class CameraSettings
{
    [Header("Basics")]
    public float distance = 1f;
    [Range(2f, 200f)] public float fieldOfView = 75f;
    public Vector3 rotationOffset;
    public Vector3 positionOffset;
    public Vector2 range;
    [Header("Lerps")]
    public float fovLerp = 1f;
    public float followLerp = 1f;
    public float camLerp = 1f;
    [Header("Speed Enhancer")]
    public float speedEffectMultiplier = 1f;
}

[CreateAssetMenu]
public class CameraSettingsAsset : ScriptableObject
{
    public CameraSettings settings;
}

public class CameraController : MonoBehaviour
{
    [Header("Referencies")]
    public Camera cam;
    public Transform target;

    [Header("Settings")]
    public CameraSettingsAsset defaultSet;
    public CameraSettings settings;

    [Header("Speed Enhancer")]
    public float maxEffect = 2f;
    public float multiplier = 1f;
    public Vector2 distanceRange;
    public Vector2 fovRange;
    
    // MOVEMENT
    float originFieldOfView;
    float originDistance;
    Vector3 originPosition;
    Vector3 originRotation;
    float currentFieldOfView;
    float currentDistance;
    Vector3 currentRotation;
    Vector3 currentPosition;
    float targetFieldOfView;
    float targetDistance;
    Vector3 targetPosition;
    Vector3 targetRotation;
    Vector3 wantedCameraPosition;

    // SPEED
    float distanceOffset;
    float fovOffset;
    Vector3 lastTargetPosition;
    // SHAKE
    private float timer = 0f;
    private float intensity = 0.7f;
    private float duration = 0f;

    void Load(CameraSettings s)
    {
        settings = s;
        // POSITION
        originPosition = settings.positionOffset;
        targetPosition = settings.positionOffset;
        currentPosition = settings.positionOffset;
        // ROTATION
        originRotation = settings.rotationOffset;
        targetRotation = settings.rotationOffset;
        currentRotation = settings.rotationOffset;
        // FOV
        originFieldOfView = settings.fieldOfView;
        targetFieldOfView = settings.fieldOfView;
        currentFieldOfView = settings.fieldOfView;
        // DISTANCE
        originDistance = settings.distance;
        targetDistance = settings.distance;
        currentDistance = settings.distance;
    }

    void Start()
    {
        Load(settings);
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
            // Follow range draw
            UnityEditor.Handles.DrawWireDisc(currentPosition, Vector3.up, settings.range.x);
            UnityEditor.Handles.DrawWireDisc(currentPosition, Vector3.up, settings.range.y);
            Gizmos.DrawLine(transform.position, currentPosition);
            Gizmos.DrawLine(currentPosition, target.position);
            UnityEditor.Handles.Label(currentPosition + Vector3.right * settings.range.x, "Min " + settings.range.x.ToString(), style);
            UnityEditor.Handles.Label(currentPosition + Vector3.right * settings.range.y, "Max " + settings.range.y.ToString(), style);
            UnityEditor.Handles.Label((transform.position + target.position)/2, "Dist " + currentDistance.ToString(), style);

            Gizmos.DrawLine(transform.position, wantedCameraPosition);
        }
        else
        {
            Gizmos.DrawLine(transform.position, targetPosition);
        }

        //Gizmos.DrawLine(transform.position,
        //    transform.position + new Vector3( transform.forward.x, 0f, transform.forward.z)
        //);
    }
    
    // Execute when editing values in the inspector
    void OnValidate()
    {
        Load(settings);

        if(settings.distance < 0f) settings.distance = 0f;
        if(settings.fieldOfView < 2f) settings.fieldOfView = 2f;
        if(targetDistance < 0) targetDistance = 0f;
        if(settings.range.y < settings.range.x) settings.range.y = settings.range.x;
        if(target != null) currentPosition += target.position;

        wantedCameraPosition = currentPosition + Quaternion.Euler(currentRotation) * Vector3.forward * currentDistance;
        transform.position = wantedCameraPosition;

        Apply();
    }
    
#endregion

    void FixedUpdate()
    {
        // Distance cannot be less than 0
        if(settings.distance < 0) settings.distance = 0;

        // Initializing values
        Vector3 offset = Vector3.zero;
        Vector3 targetMovementDirection = Vector3.zero;
        Vector3 camDirection = new Vector3( transform.forward.x, 0f, transform.forward.z);
        float distanceFromTarget = 0f;
        float targetMovementVelocity = 0f;
        float speed = 1f;

        // If Camera if following a target
        if(target != null) 
        {
            offset = target.position;
            
            // Getting usefull values about target
            distanceFromTarget = Vector3.Distance(currentPosition, target.position);
            targetMovementVelocity = Vector3.Distance(target.position, lastTargetPosition);
            targetMovementDirection = (target.position - lastTargetPosition).normalized;

            // Increasing the position lerp if the target go further the distanceRange
            speed = 1f + distanceFromTarget/distanceRange.y;

            // The Speed Enhancement effect
            float ratio = Mathf.Clamp(targetMovementVelocity / maxEffect, 0f, 1f);
            fovOffset = fovRange.x + (fovRange.y - fovRange.x) * ratio * multiplier;
            distanceOffset = distanceRange.x + (distanceRange.y - distanceRange.x) * ratio * multiplier;
            
            lastTargetPosition = target.position;
        }

        currentPosition = Vector3.Lerp(currentPosition, targetPosition + offset, Time.deltaTime * settings.followLerp * speed);
        currentFieldOfView = Mathf.Lerp(cam.fieldOfView, targetFieldOfView + fovOffset, Time.deltaTime * settings.fovLerp);
        currentDistance = targetDistance + distanceOffset;
        Vector3 angleVector = new Vector3
            (0f, 
            Vector3.SignedAngle(
                new Vector3(
                    targetMovementDirection.x,
                    targetMovementDirection.y,
                    -targetMovementDirection.z
                ), 
                Vector3.forward,
                Vector3.up),
            0f);
        currentRotation = targetRotation + angleVector;

        // Applying current values
        wantedCameraPosition = currentPosition + Quaternion.Euler(currentRotation) * Vector3.forward * currentDistance;
        Apply();

        // Shake
        if(timer > 0)
        {
            timer -= Time.deltaTime;
            currentRotation += Random.insideUnitSphere * intensity;
            intensity *= timer/duration;
            if(timer <= 0) Teleport();
        }
    }

    [ContextMenu("Shake")]
    public void DEBUG_Shake() {Shake(5f, 2f);}
    public void Shake(float i = 0.5f, float d = 1f)
    {   
        intensity = i;
        duration = d;
        timer = duration;
    }

    void Apply()
    {
        transform.forward = -(transform.position - currentPosition).normalized;
        transform.position = Vector3.Lerp(transform.position, wantedCameraPosition, Time.deltaTime * settings.camLerp);
        cam.fieldOfView = currentFieldOfView;
    } // Apply the values to the camera 

    public void Focus(Vector3 newPosition, CameraSettings set = null)
    {
        targetPosition = newPosition;
        target = null;
        if(set != null) Load(set);
    } // Focus camera on a new position (Vector3)

    public void Focus(Transform newTarget, CameraSettings set = null)
    {
        target = newTarget;
        if(set != null) Load(set);
    } // Focus camera on a new target (transform)

    public void Teleport()
    {
        currentPosition = targetPosition;
        currentRotation = targetRotation;
        cam.fieldOfView = targetFieldOfView;
        Apply();
    } // Teleport all the camera values instantly (to ignore lerp)

    public void Reset()
    {
        target = null;
        targetPosition = originPosition;
        targetRotation = originRotation;
        targetFieldOfView = originDistance;
        targetDistance = originDistance;
    } // Reset all the values to the origin values
}