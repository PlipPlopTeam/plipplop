using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Referencies")]
    public Camera cam;
    public Transform target;
    [Header("Settings")]
    public float distance = 1f;
    public Vector3 positionOffset;
    public float positionLerpSpeed = 5f;
    public float rotationLerpSpeed = 1f;
    [Header("FOV")]
    public float fieldOfView = 75f;
    public float fovLerpSpeed = 2f;
    float originFOV;

    float originDistance;
    Vector3 originPosition;
    Vector3 targetPosition;
    Vector3 directionToPivot;
    Quaternion targetRotation;
    Quaternion originRotation;

    void Start()
    {
        directionToPivot = (cam.transform.position - transform.position).normalized;
        originDistance = (cam.transform.position - transform.position).magnitude;
        originPosition = transform.position;
        originRotation = cam.transform.rotation;
        distance = originDistance;
        originFOV = cam.fieldOfView = fieldOfView;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color32(173, 216, 230, 255);

        if(target)
        {
            UnityEditor.Handles.DrawWireDisc(
                transform.position,
                transform.up, 
                0.5f
            );
            Gizmos.DrawLine(transform.position, target.position);
            UnityEditor.Handles.DrawWireDisc(
                target.position,
                target.up, 
                1f
            );
        }
    }

    // Execute when editing values in the inspector
    void OnValidate()
    {
        directionToPivot = (cam.transform.position - transform.position).normalized;
        
        if(directionToPivot == Vector3.zero)
            directionToPivot = -cam.transform.forward;

        if(target != null)
            transform.position = target.position + positionOffset;

        if(distance < 0)
            distance = 0;
        
        cam.transform.localPosition = directionToPivot * distance;
        
        if(cam.transform.localPosition.z > 0) 
            cam.transform.localPosition = new Vector3(cam.transform.localPosition.x, cam.transform.localPosition.y, 0f);

        cam.fieldOfView = fieldOfView;
    }

    void Update()
    {
        // POSITION
        if(target != null)
            transform.position = Vector3.Lerp(transform.position, target.position + positionOffset, Time.deltaTime * positionLerpSpeed);
            //transform.forward = Vector3.Lerp(transform.forward, target.forward, Time.deltaTime * rotationLerpSpeed);

        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * rotationLerpSpeed);

        // DISTANCE
        cam.transform.localPosition = Vector3.Lerp(cam.transform.localPosition, directionToPivot * distance, Time.deltaTime * positionLerpSpeed);
        
        // FOV
        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, fieldOfView, Time.deltaTime * fovLerpSpeed);
    }

    public void JumpTo(Vector3 newPoint, float newDistance)
    {
        transform.position = newPoint;
        cam.transform.localPosition = directionToPivot * distance;
    }

    public void FocusOn(Vector3 newPosition, float newDistance)
    {
        targetPosition = newPosition;
        target = null;
        distance = newDistance;
    }
    public void FocusOn(Transform newTransform, float newDistance)
    {
        target = newTransform;
        targetPosition = Vector3.zero;
        distance = newDistance;
    }

    public void ZoomOn(Transform newTransform, float fov)
    {
        target = newTransform;
        targetPosition = Vector3.zero;
        fieldOfView = fov;
    }

    public void ResetFov()
    {
        fieldOfView = originFOV;
    }

    public void Reset()
    {
        target = null;
        targetPosition = originPosition;
        distance = originDistance;
    }
}