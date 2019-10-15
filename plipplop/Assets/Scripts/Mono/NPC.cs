using UnityEngine;
using UnityEngine.AI;

public class NPC : MonoBehaviour
{
    public Sight sight;
    public FocusLook look;
    public NavMeshAgent agent;
    public Animator animator;

    Transform t;

    void Update()
    {
        GameObject[] seens = sight.Scan();
        foreach(GameObject go in seens)
        {
            Controller c = go.GetComponent<Controller>();
            if(c != null) 
            {
                look.FocusOn(c.transform);
                t = c.transform;
                return;
            }
        }

        t = null;
    }

    void OnDrawGizmos()
    {
        if(t != null)
            Gizmos.DrawLine(transform.position + sight.offset, t.position);
    }
}
