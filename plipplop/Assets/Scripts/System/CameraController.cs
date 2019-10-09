using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Referencies")]
    public Camera cam;
    [Header("Settings")]
    public float distance = 1f;
    public float fieldOfView = 75f;
    [Space(15)]
    public Transform target;
    public Vector3 rotation;
    public Vector3 position;
    [Header("Lerps")]
    public float fovLerpSpeed = 2f;
    public float positionLerpSpeed = 5f;
    public float rotationLerpSpeed = 1f;

    // MOVEMENT
    float originFOV;
    float originDistance;
    Vector3 originPosition;
    Vector3 originRotation;
    Vector3 currentRotation;
    Vector3 currentPosition;
    // SHAKE
    private float timer = 0f;
    private float intensity = 0.7f;
    private float duration = 0f;

    void Start()
    {
        currentPosition = position;
        position = currentPosition;
        currentRotation = rotation;
        originRotation = currentRotation;
        cam.fieldOfView = fieldOfView;
        originFOV = cam.fieldOfView;
        originDistance = distance;
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
            Gizmos.DrawLine(transform.position, position);
            Gizmos.DrawWireCube(position, Vector3.one/10f);
        }
    }
    
    // Execute when editing values in the inspector
    void OnValidate()
    {
        currentPosition = position;
        currentRotation = rotation;
        cam.fieldOfView = fieldOfView;
        if(distance < 0) distance = 0;
        if(target != null) currentPosition += target.position;
        Apply();
    }
#endregion

    void Update()
    {
        Vector3 offset = Vector3.zero;
        if(distance < 0) distance = 0;
        if(target != null) offset = target.position;

        // Lerping current values
        currentPosition = Vector3.Lerp(currentPosition, position + offset, Time.deltaTime * positionLerpSpeed);
        currentRotation = Vector3.Lerp(currentRotation, rotation, Time.deltaTime * rotationLerpSpeed);
        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, fieldOfView, Time.deltaTime * fovLerpSpeed);

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
        transform.position = currentPosition + Quaternion.Euler(currentRotation) * Vector3.forward * distance;
        cam.fieldOfView = fieldOfView;
    } // Apply the values to the camera 

    public void Focus(Vector3 newPosition, float newDistance)
    {
        position = newPosition;
        target = null;
        distance = newDistance;
    } // Focus camera on a new position (Vector3)

    public void Focus(Transform newTarget, float newDistance)
    {
        target = newTarget;
        position = Vector3.zero;
        distance = newDistance;
    } // Focus camera on a new target (transform)

    public void Teleport()
    {
        currentPosition = position;
        currentRotation = rotation;
        cam.fieldOfView = fieldOfView;
        Apply();
    } // Teleport all the camera values instantly (to ignore lerp)

    public void ResetFov()
    {
        fieldOfView = originFOV;
    } // Reset the FOV to the origin FOV

    public void Reset()
    {
        target = null;
        position = originPosition;
        rotation = originRotation;
        distance = originDistance;
    } // Reset all the values to the origin values
}