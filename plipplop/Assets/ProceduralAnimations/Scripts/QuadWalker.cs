using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuadWalker : MonoBehaviour
{

    public LegStepper left;
    public LegStepper right;

    public LegStepper bLeft;
    public LegStepper bRight;

    public Transform body;

    public Vector2 heightLimits;
    public float bopLerpSpeed;

    public float maxFootTargetDistance;
    public Transform footTargetTransform;
    public QuadController QuadController;

    private Vector3 footTargetStartPosition;
    public LayerMask mask;
    
    void Awake()
    {
        footTargetStartPosition = footTargetTransform.localPosition;
        if (bLeft && bRight)
        {
            StartCoroutine(LegUpdateCoroutineQuadri());
        }
        else
        {
            StartCoroutine(LegUpdateCoroutine());
        }
    }
    
    IEnumerator LegUpdateCoroutine()
    {
        while (true)
        {
            do
            {
                left.TryMove(!QuadController.walking && QuadController.turning);
                yield return null;
            } while (left.Moving );

            do
            {
                right.TryMove(!QuadController.walking && QuadController.turning);
                yield return null;
            } while (right.Moving);
        }
    }

    IEnumerator LegUpdateCoroutineQuadri()
    {
        while (true)
        {
            do
            {
                left.TryMove(!QuadController.walking && QuadController.turning);
                bRight.TryMove(!QuadController.walking && QuadController.turning);
                yield return null;
            } while (left.Moving || bRight.Moving );

            do
            {
                right.TryMove(!QuadController.walking && QuadController.turning);
                bLeft.TryMove(!QuadController.walking && QuadController.turning);
                yield return null;
            } while (right.Moving ||bLeft.Moving);
        }
    }

    private void FixedUpdate()
    {
       // Rest();
        //SetFootTargetPosition();
    }

    public void BodyBop()
    {

        body.localPosition = new Vector3(0, heightLimits.x, 0);

    }

    void Rest()
    {
        body.localPosition = Vector3.Lerp(body.localPosition, new Vector3(0, heightLimits.y, 0), bopLerpSpeed);

    }

    void SetFootTargetPosition()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position, Vector3.down + (QuadController.rb.velocity/QuadController.speed) * maxFootTargetDistance, out hit, 10f,mask))
        {
            footTargetTransform.position = hit.point;
        }
        else
        {
            footTargetTransform.localPosition = footTargetStartPosition;
        }
    }
    
    
}
