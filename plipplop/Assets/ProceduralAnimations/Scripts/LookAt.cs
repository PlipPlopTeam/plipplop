using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAt : MonoBehaviour
{

    [Header("HEAD")] 
    
    public bool lookAtHead = true;
    public Transform headBone;
    public Transform target;

    public float headMaxTurnAngle = 0;
    public float headTrackingSpeed = 0;

    [Header("EYES")] 
    
    public bool lookAtEyes = true;

    public bool crossEyed = false;
    
    public Transform leftEyeBone;
    public Transform rightEyeBone;

    public float eyeTrackingSpeed;
    public float leftEyeMaxYRotation;
    public float leftEyeMinYRotation;
    public float rightEyeMaxYRotation;
    public float rightEyeMinYRotation;

    public List<Transform> potentialTargets;
    
    private void LateUpdate()
    {
        if(lookAtHead && target)
        HeadLookAt();
        
        if(lookAtEyes)
        EyeLookAt();

        target = GetCloserTarget();

    }

    Transform GetCloserTarget()
    {
        float _d = 1000;
        Transform _t = null;

        foreach (var _pt in potentialTargets)
        {
            if (Vector3.Distance(transform.position, _pt.position) < _d)
            {
                _t = _pt;
            }
        }

        return _t;
    }

    void HeadLookAt()
    {
        Quaternion currentLocalRotation = headBone.localRotation;
       
        headBone.localRotation = Quaternion.identity;

        Vector3 targetWorldLookDir = target.position - headBone.position;
        Vector3 targetLocalLookDir = headBone.InverseTransformDirection(targetWorldLookDir);

        targetLocalLookDir = Vector3.RotateTowards(
            Vector3.forward,
            targetLocalLookDir,
            Mathf.Deg2Rad * headMaxTurnAngle, 
            0 
        );

        Quaternion targetLocalRotation = Quaternion.LookRotation(targetLocalLookDir, Vector3.up);

        headBone.localRotation = Quaternion.Slerp(
            currentLocalRotation,
            targetLocalRotation, 
            1 - Mathf.Exp(-headTrackingSpeed * Time.deltaTime)
        );
    }

    void EyeLookAt()
    {
        if (crossEyed)
        {
            

            leftEyeBone.rotation = Quaternion.Slerp(
                leftEyeBone.rotation,
                Quaternion.LookRotation(
                    target.position - leftEyeBone.position, 
                    transform.up
                ),
                1 - Mathf.Exp(-eyeTrackingSpeed * Time.deltaTime)
            );

            rightEyeBone.rotation = Quaternion.Slerp(
                rightEyeBone.rotation,
                Quaternion.LookRotation(
                    target.position - rightEyeBone.position, 
                    transform.up
                ),
                1 - Mathf.Exp(-eyeTrackingSpeed * Time.deltaTime)
            );
        }
        else
        {
            Quaternion targetEyeRotation = Quaternion.LookRotation(
                target.position - headBone.position, 
                transform.up
            );

            leftEyeBone.rotation = Quaternion.Slerp(
                leftEyeBone.rotation,
                targetEyeRotation,
                1 - Mathf.Exp(-eyeTrackingSpeed * Time.deltaTime)
            );

            rightEyeBone.rotation = Quaternion.Slerp(
                rightEyeBone.rotation,
                targetEyeRotation,
                1 - Mathf.Exp(-eyeTrackingSpeed * Time.deltaTime)
            );
        }
        
       
        

        float leftEyeCurrentYRotation = leftEyeBone.localEulerAngles.y;
        float rightEyeCurrentYRotation = rightEyeBone.localEulerAngles.y;

        if (leftEyeCurrentYRotation > 180)
        {
            leftEyeCurrentYRotation -= 360;
        }
        if (rightEyeCurrentYRotation > 180) 
        {
            rightEyeCurrentYRotation -= 360;
        }

        float leftEyeClampedYRotation = Mathf.Clamp(
            leftEyeCurrentYRotation,
            leftEyeMinYRotation,
            leftEyeMaxYRotation
        );
        float rightEyeClampedYRotation = Mathf.Clamp(
            rightEyeCurrentYRotation,
            rightEyeMinYRotation,
            rightEyeMaxYRotation
        );

        leftEyeBone.localEulerAngles = new Vector3(
            leftEyeBone.localEulerAngles.x,
            leftEyeClampedYRotation,
            leftEyeBone.localEulerAngles.z
        );
        rightEyeBone.localEulerAngles = new Vector3(
            rightEyeBone.localEulerAngles.x,
            rightEyeClampedYRotation,
            rightEyeBone.localEulerAngles.z
        );
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Interesting"))
        {
            potentialTargets.Add(other.transform);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Interesting"))
        {
            potentialTargets.Remove(other.transform);
        }
    }
}
