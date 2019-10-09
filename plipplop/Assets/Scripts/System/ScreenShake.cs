using UnityEngine;

public class ScreenShake : MonoBehaviour
{
    public static ScreenShake instance;
    public Transform cameraTransform;
    // SETTINGS
    private float timer = 0f;
    private float intensity = 0.7f;
    private float duration = 0f;
    Vector3 originalPos;

    void Awake()
    {
        instance = this;
        SetCamera(cameraTransform);
    }

    public void SetCamera(Transform camTrans)
    {
        cameraTransform = camTrans;
        originalPos = cameraTransform.localPosition;
    }

    public void Shake(float i = 0.5f, float d = 1f)
    {   
        intensity = i;
        duration = d;
        timer = duration;
    }

    void Update()
    {
        if(timer > 0)
        {
            cameraTransform.localPosition = originalPos + Random.insideUnitSphere * intensity;
            timer -= Time.deltaTime;
            intensity *= timer/duration;
            if(cameraTransform.localPosition != originalPos && timer <= 0) cameraTransform.localPosition = originalPos;
        }
    }
}