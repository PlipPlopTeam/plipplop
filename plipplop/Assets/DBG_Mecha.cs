using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DBG_Mecha : MonoBehaviour
{
    public new Camera camera;
    public float stretch;
    public Transform[] legs;

    public bool isWalking { get { return walkAccumulator > 0f; } }
    public float accumulationSpeed = 1f;
    public float maxSpeed = 10f;
    public float legsSpeed = 3f;
    public float turnSpeed = 1f;
    public float turnMaxSpeed = 5f;
    public float turnSidePush = 2f;
    public float walkAmplitude = 10f;
    public float shakeForce = 0.3f;
    public float goDownForce = 10f;

    float turnAccumulator = 0f;
    float walkAccumulator = 0f;
    float legsAccumulator = 0f;

    Geometry.PositionAndRotation originalCameraSettings;
    Geometry.PositionAndRotation positionWithoutShake;
    Geometry.PositionAndRotation positionWithShake;

    // Start is called before the first frame update
    void Start()
    {
        originalCameraSettings = new Geometry.PositionAndRotation() { position = camera.transform.localPosition, rotation = camera.transform.localRotation };
        positionWithoutShake = new Geometry.PositionAndRotation() { position = camera.transform.localPosition, rotation = camera.transform.localRotation };
        positionWithShake = new Geometry.PositionAndRotation() { position = camera.transform.localPosition, rotation = camera.transform.localRotation };
    }

    // Update is called once per frame
    void Update()
    {

        // ADVANCE

        if (Input.GetKey(KeyCode.Z))
        {
            walkAccumulator = Mathf.Clamp(walkAccumulator + Time.deltaTime * accumulationSpeed, 0f, 1f);
        }
        else
        {
            walkAccumulator = Mathf.Clamp(walkAccumulator - Time.deltaTime * accumulationSpeed, 0f, 1f);
        }

        // TURN

        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.Q))
        {
            turnAccumulator = Mathf.Clamp(turnAccumulator + Time.deltaTime * turnSpeed * (Input.GetKey(KeyCode.D) ? 1f : -1f), -1f, 1f);
        }
        else if (turnAccumulator != 0f)
        {
            turnAccumulator = Mathf.Clamp(turnAccumulator + Time.deltaTime * turnSpeed * (-Mathf.Sign(turnAccumulator)), -1f, 1f);
            turnAccumulator = Mathf.Round(turnAccumulator * 100f) / 100f;
        }



        // ANIM
        positionWithoutShake.position = Vector3.Lerp(positionWithoutShake.position, originalCameraSettings.position, 10f*Time.deltaTime);
        if (walkAccumulator > 0f || turnAccumulator > 0f)
        {
            if (legs[0].localEulerAngles.x == 0)
            {
                legs[0].localEulerAngles = Vector3.right * walkAmplitude;
                legs[1].localEulerAngles = Vector3.left * walkAmplitude;
            }
            legsAccumulator += walkAccumulator + Mathf.Abs( turnAccumulator)*0.5f;
            if (legsAccumulator % legsSpeed < legsAccumulator)
            {
                legsAccumulator = 0f;
                legs[0].localEulerAngles *= -1f;
                legs[1].localEulerAngles *= -1f;
                positionWithoutShake.position += goDownForce * Vector3.down;
                StartCoroutine(Shake());

            }
        }
        else
        {
            legs[0].localEulerAngles = Vector3.zero;
            legs[1].localEulerAngles = Vector3.zero;
            legsAccumulator = 0f;
        }

        transform.Translate(Vector3.forward * walkAccumulator * maxSpeed * Time.deltaTime 
            + Vector3.left * turnAccumulator * turnSidePush * walkAccumulator);
        transform.Rotate(Vector3.up * turnAccumulator * turnMaxSpeed);

        camera.transform.localPosition = positionWithShake.position;
        camera.transform.localRotation = positionWithShake.rotation;

    }

    IEnumerator Shake()
    {
        float remainingSeconds = 0.14f;
        while(remainingSeconds > 0)
        {
            positionWithShake.position = positionWithoutShake.position;
            positionWithShake.position += new Vector3(Random.value-0.5f, Random.value - 0.5f, Random.value - 0.5f)*shakeForce;
            remainingSeconds -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        positionWithShake.position = positionWithoutShake.position;
    }
}
