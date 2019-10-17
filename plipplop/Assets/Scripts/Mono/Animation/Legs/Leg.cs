using UnityEngine;

public class Leg : MonoBehaviour
{
    [Header("Referencies")]
    public Legs body;
    public LayerMask raycastMask;
    public Transform foot;
    public Transform hip;
    public Transform knee;
    [Header("Settings")]
    public float forwardDistance = 1;
    public float maxFootDistance = 2;
    public float kneeNoise = .2f;
    public float kneeVelInfluence = 0;

    private Vector3 kneeOffset;
    
    private void Start()
    {
        foot.transform.parent = null;
    }

    void Update()
    {
        if (Vector3.Distance(foot.position, hip.position) > maxFootDistance)
        {
            foot.SetParent(hip);
            UpdateKnee(body.velocity);
        }
        
        if (foot.parent != null)
        {
            foot.position = Vector3.Lerp(foot.position, hip.position + Vector3.down / 1.5f, Time.deltaTime * 10);
            UpdateKnee(body.velocity, 0);
        }
        else
        {
            UpdateKnee(body.velocity);
        }
    }
    
    public void UpdateLeg(Vector3 _vel)
    {
        _vel.y = 0;
        _vel = Vector3.ClampMagnitude(_vel, 1);
        RaycastHit[] hits = Physics.RaycastAll(transform.position, -Vector3.up + Vector3.ClampMagnitude(body.velocity, 1f), maxFootDistance, raycastMask);
        foreach(RaycastHit hit in hits)
        {
            if(!hit.collider.isTrigger && hit.transform != body.body)
            {
                foot.position = hit.point ;
                foot.transform.up = hit.normal;
                foot.transform.eulerAngles = new Vector3(foot.transform.eulerAngles.x, transform.eulerAngles.y - 90 , foot.transform.eulerAngles.z);
                if(foot.parent != null) foot.parent = null;
            }
        }
        kneeOffset = GetNoise();
        UpdateKnee(_vel); 
    }

    public void PullLeg()
    {
        //foot.localPosition = Vector3.down / 2f;
        //knee.localPosition = Vector3.down / 4f + Vector3.right;
        //foot.transform.up = hip.transform.forward;
    }

    public void UpdateKnee(Vector3 _vel, float _noise = 1)
    {
        _vel = Vector3.ClampMagnitude(_vel, 1);
        _vel.y = 0;
        knee.position = (hip.position + foot.position) / 2 + kneeOffset * _noise + _vel * kneeVelInfluence;
    }

    Vector3 GetNoise(float _y = 1)
    {
        return new Vector3(Random.Range(-kneeNoise, kneeNoise), Random.Range(-kneeNoise, kneeNoise) * _y,
            Random.Range(-kneeNoise, kneeNoise));
    }
}
