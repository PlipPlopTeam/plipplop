using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class LegStepper : MonoBehaviour
{
    // The position and rotation we want to stay in range of
    [SerializeField] Transform homeTransform;
    // Stay within this distance of home
    [SerializeField] float wantStepAtDistance;
    // How long a step takes to complete
    [SerializeField] float moveDuration;
    
    [SerializeField] float stepOvershootFraction;

    public float positionVariation = .3f;

    private Vector3 homeTransformStartPosition;

    public QuadWalker QuadWalker;

    public Transform parentTransform;
  
    // Is the leg moving?
    public bool Moving;

    private void Start()
    {
        homeTransformStartPosition = homeTransform.localPosition;
    }

    public void TryMove(bool _forced = false)
    {
        // If we are already moving, don't start another move
        if (Moving) return;
        
        SetFootTargetPosition();


        float distFromHome = Vector3.Distance(transform.position, homeTransform.position);

        // If we are too far off in position or rotation
        if (distFromHome > wantStepAtDistance || _forced)
        {
            // Start the step coroutine
            StartCoroutine(Move());
        }
    }
    
    IEnumerator Move()
    {
        Son.I.PlaySound(0, false);
        Moving = true;
        
        //walker.BodyBop();

        Vector3 startPoint = transform.position;
        Quaternion startRot = transform.rotation;

        Quaternion endRot = homeTransform.rotation;

        // Directional vector from the foot to the home position
        Vector3 towardHome = (homeTransform.position - transform.position);
        // Total distnace to overshoot by   
        float overshootDistance = wantStepAtDistance * stepOvershootFraction;
        Vector3 overshootVector = towardHome * overshootDistance;
        // Since we don't ground the point in this simplified implementation,
        // we restrict the overshoot vector to be level with the ground
        // by projecting it on the world XZ plane.
        overshootVector = Vector3.ProjectOnPlane(overshootVector, Vector3.up);

        // Apply the overshoot
        Vector3 endPoint = homeTransform.position + overshootVector 
                                                  + new Vector3(
                                                      Random.Range(-positionVariation/2, positionVariation/2),
                                                      0,
                                                      Random.Range(-positionVariation/2, positionVariation/2)
                                                      );

        // We want to pass through the center point
        Vector3 centerPoint = (startPoint + endPoint) / 2;
        // But also lift off, so we move it up by half the step distance (arbitrarily)
        centerPoint += homeTransform.up * Vector3.Distance(startPoint, endPoint) / 2f;

        float timeElapsed = 0;
        do
        {
            timeElapsed += Time.deltaTime;
            float normalizedTime = timeElapsed / moveDuration;
            normalizedTime = Easing.Cubic.InOut(normalizedTime);


            // Quadratic bezier curve
            transform.position =
                Vector3.Lerp(
                    Vector3.Lerp(startPoint, centerPoint, normalizedTime),
                    Vector3.Lerp(centerPoint, endPoint, normalizedTime),
                    normalizedTime
                );

            transform.rotation = Quaternion.Slerp(startRot, endRot, normalizedTime);

            yield return null;
        }
        while (timeElapsed < moveDuration);

        Moving = false;
        Son.I.PlaySound(1, false);

    }
    
    void SetFootTargetPosition()
    {
        RaycastHit hit;

        if (Physics.Raycast(parentTransform.position, Vector3.down + (QuadWalker.QuadController.rb.velocity/QuadWalker.QuadController.speed) * QuadWalker.maxFootTargetDistance, out hit, 10f,QuadWalker.mask))
        {
            homeTransform.position = hit.point;
        }
        else
        {
            homeTransform.localPosition = homeTransformStartPosition;
        }
    }
}
