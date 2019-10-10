using UnityEngine;

[System.Serializable]
public class CameraSettings
{
    [Header("Basics")]
    public float distance = 1f;
    [Range(2f, 200f)] public float fieldOfView = 75f;
    public Vector3 rotation;
    public Vector3 offset;
    [Header("Lerps")]
    public float fovLerpSpeed = 1f;
    public float positionLerpSpeed = 1f;
    public float rotationLerpSpeed = 1f;
    public float distanceLerpSpeed = 1f;
    [Header("Speed Enhancer")]
    public float speedEffectMultiplier = 1f;
    [Header("Follow")]
    public Vector2 range;
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

    // SPEED
    float distanceOffset;
    Vector3 lastTargetPosition;
    // SHAKE
    private float timer = 0f;
    private float intensity = 0.7f;
    private float duration = 0f;

    void Load(CameraSettings s)
    {
        settings = s;
        // POSITION
        originPosition = settings.offset;
        targetPosition = settings.offset;
        currentPosition = settings.offset;
        // ROTATION
        originRotation = settings.rotation;
        targetRotation = settings.rotation;
        currentRotation = settings.rotation;
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
            UnityEditor.Handles.Label((transform.position + target.position)/2, "Dist " + settings.distance.ToString(), style);
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

        // If Camera if following a target
        if(target != null) 
        {
            offset = target.position;
            distanceFromTarget = Vector3.Distance(currentPosition, target.position);
            targetMovementVelocity = Vector3.Distance(target.position, lastTargetPosition);
            float ratio = Mathf.Clamp(targetMovementVelocity / maxEffect, 0f, 1f);

            targetFieldOfView = fovRange.x + (fovRange.y - fovRange.x) * ratio * multiplier;
            distanceOffset = distanceRange.x + (distanceRange.y - distanceRange.x) * ratio * multiplier;

            targetMovementDirection = (target.position - lastTargetPosition).normalized;

            lastTargetPosition = target.position;
        }

        // Lerping current values
        currentDistance = Mathf.Lerp(currentDistance, targetDistance/* + distanceOffset*/, Time.deltaTime * settings.distanceLerpSpeed);
        currentRotation = Vector3.Lerp(currentRotation, targetRotation, Time.deltaTime * settings.rotationLerpSpeed);
        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetFieldOfView, Time.deltaTime * settings.fovLerpSpeed);

        if(distanceFromTarget > settings.range.x)
        {   
            float s = (distanceFromTarget - settings.range.x) / settings.range.y;
            currentPosition = Vector3.Lerp(currentPosition, targetPosition + offset, Time.deltaTime * settings.positionLerpSpeed * s);

            if(targetMovementDirection.magnitude > 0.1f)
            {
                targetRotation = 
                new Vector3(
                    targetRotation.x,
                    Vector3.SignedAngle(
                        new Vector3(targetMovementDirection.x, targetMovementDirection.y, -targetMovementDirection.z),
                        Vector3.forward,
                        Vector3.up
                    ),
                targetRotation.z);
            }
        }

        // Shake
        if(timer > 0)
        {
            timer -= Time.deltaTime;
            currentRotation += Random.insideUnitSphere * intensity;
            intensity *= timer/duration;
            if(timer <= 0) Teleport();
        }

        // Applying current values
        Apply();
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
        transform.position = currentPosition + Quaternion.Euler(currentRotation) * Vector3.forward * currentDistance;
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