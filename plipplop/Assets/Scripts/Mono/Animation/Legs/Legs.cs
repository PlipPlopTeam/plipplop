using System.Collections;
using UnityEngine;

public class Legs : MonoBehaviour
{
    public bool lerpLeg;
    public bool rightLeg;
    public bool lerpBody = true;
    public Leg[] legs;
    public Vector3 velocity = Vector3.zero;
    public Transform body;

    public float legFps = 4;
    public float bodyAmp = .05f;
    public float bodyTilt = 1;
    public float bodyTurn = 10;

    public bool isJumping = false;

    Vector3 bodyStartPosition;
    Vector3 startLocalEulerAngles;
    float timer;
    
    void Start()
    {
        bodyStartPosition = body.localPosition;
        startLocalEulerAngles = body.localEulerAngles;
    }

    public void Update()
    {
        if(timer > 0) timer -= Time.deltaTime;
        else
        {
            UpdateLegs();
            timer = 1/legFps;
        }
    }

    void UpdateLegs()
    {
        if (rightLeg)
        {
            legs[0].UpdateLeg(velocity);        
        }
        else
        {
            legs[1].UpdateLeg(velocity);
        }
        
        rightLeg = !rightLeg;
    }

    private void OnEnable()
    {
        foreach (var leg in legs) {
            leg.foot.position = leg.hip.position + Vector3.down / 1.5f;
            leg.UpdateKnee(Vector3.zero);
        };
    }
}
