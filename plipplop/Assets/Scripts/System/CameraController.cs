using UnityEngine;

[CreateAssetMenu]
public class CameraSettings : ScriptableObject
{
    [Header("Basics")]
    public float distance = 1f;
    public float fieldOfView = 75f;
    public Vector3 rotation;
    public Vector3 offset;
    [Header("Lerps")]
    public float fovLerpSpeed = 1f;
    public float positionLerpSpeed = 1f;
    public float rotationLerpSpeed = 1f;
    public float distanceLerpSpeed = 1f;
    [Header("Speed Enhancer")]
    public float speedEffectMultiplier = 1f;
}

public class CameraController : MonoBehaviour
{
    [Header("Referencies")]
    public Camera cam;
    public CameraSettings defaultSettings;
    public Transform target;

    [Header("Speed Enhancer")]
    public float maxEffect = 2f;
    public float multiplier = 1f;
    public Vector2 distanceRange;
    public Vector2 fovRange;
    
    // MOVEMENT
    CameraSettings settings;
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

        originPosition = targetPosition = currentPosition = settings.offset;
        originRotation = targetRotation = currentRotation = settings.rotation;
        originFieldOfView = targetFieldOfView = currentFieldOfView = settings.fieldOfView;
        originDistance = targetDistance = currentDistance = settings.distance;
    }

    void Start()
    {
        Load(defaultSettings);
    }

#region INSPECTOR 
    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color32(173, 216, 230, 255);
        if(target)
        {
            UnityEditor.Handles.DrawWireDisc(transform.position, transform.up, 0.5f);
            Gizmos.DrawLine(transform.position, target.position);
            UnityEditor.Handles.DrawWireDisc(target.position, target.up, 1f);
        }
        else
        {
            Gizmos.DrawLine(transform.position, targetPosition);
            Gizmos.DrawWireCube(targetPosition, Vector3.one/10f);
        }
    }
    
    // Execute when editing values in the inspector
    //void OnValidate()
    /*
    {
        //Load(defaultSettings);
        if(targetDistance < 0) targetDistance = 0;
        if(target != null) currentPosition += target.position;
        Apply();
    }
    */
#endregion

    void Update()
    {
        Vector3 offset = Vector3.zero;
        if(settings.distance < 0) settings.distance = 0;
        if(target != null) 
        {
            offset = target.position;

            float d = Vector3.Distance(target.position, lastTargetPosition);
            lastTargetPosition = target.position;
            float ratio = Mathf.Clamp(d / maxEffect, 0f, 1f);
            targetFieldOfView = fovRange.x + (fovRange.y - fovRange.x) * ratio * multiplier;
            distanceOffset = distanceRange.x + (distanceRange.y - distanceRange.x) * ratio * multiplier;
        }

        // Lerping current values
        currentDistance = Mathf.Lerp(currentDistance, targetDistance + distanceOffset, Time.deltaTime * settings.distanceLerpSpeed);
        currentPosition = Vector3.Lerp(currentPosition, targetPosition + offset, Time.deltaTime * settings.positionLerpSpeed);
        currentRotation = Vector3.Lerp(currentRotation, targetRotation, Time.deltaTime * settings.rotationLerpSpeed);
        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetFieldOfView, Time.deltaTime * settings.fovLerpSpeed);

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