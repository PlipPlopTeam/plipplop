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
    public float scaleScale = 1f;
    public float talkWobbleSpeedVariation = 0.5f;
    public float talkWobbleBaseSpeed = 1f;

    bool up;
    Vector3 bodyStartPosition;
    Vector3 startLocalEulerAngles;
    float timer;
    
    void Start()
    {
        bodyStartPosition = body.localPosition;
        startLocalEulerAngles = body.localEulerAngles;
    }

    public void FixedUpdate()
    {
        if(timer > 0) timer -= Time.deltaTime;
        else
        {
            UpdateBody();
            UpdateLegs();
            timer = 1/legFps;
        }
    }

    void UpdateBody()
    {
        if (lerpBody)
        {
            if (up)
            {
                body.localPosition = bodyStartPosition - new Vector3(0,bodyAmp,0) * (Vector3.ClampMagnitude(velocity,1).magnitude +.1f);
            }
            else
            {
                body.localPosition = bodyStartPosition + new Vector3(0,bodyAmp,0) * (Vector3.ClampMagnitude(velocity,1).magnitude +.1f);
            }
            up = !up;
        }
    }

    void UpdateLegs()
    {
        if (rightLeg)
        {
            legs[0].UpdateLeg(velocity);
            if(lerpBody) body.localEulerAngles = new Vector3(Random.Range(-bodyTilt,bodyTilt),Random.Range(-bodyTurn,-bodyTurn + .5f),Random.Range(-bodyTilt,bodyTilt))* (Vector3.ClampMagnitude(velocity,1).magnitude + .1f);
        }
        else
        {
            legs[1].UpdateLeg(velocity);
            if(lerpBody) body.localEulerAngles = new Vector3(Random.Range(-bodyTilt,bodyTilt),Random.Range(bodyTurn,bodyTurn -5f),Random.Range(-bodyTilt,bodyTilt))* (Vector3.ClampMagnitude(velocity,1).magnitude + .1f);
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
