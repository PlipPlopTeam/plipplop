using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AgentMovement : MonoBehaviour
{
    [System.Serializable]
    public class Path
    {
        public bool loop = true;
        public int index = 0;
        public Vector3[] points;
    }
    [System.Serializable]
    public class Settings
    {
        public float speed = 5f;
        public float maxSpeed = 1f;
        public float velocityLerpSpeed = 1f;
        public float navTreshold = 1f;  
    }

    public System.Action onDestinationReached;
    public AgentMovement.Path path;
    public AgentMovement.Settings settings;

    Transform chaseTarget;
    [HideInInspector] public bool going = false;
    [HideInInspector] public bool reached = false;
    [HideInInspector] public Animator animator;

    NavMeshAgent agent;
    public void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        going = false;
    }

    public void Chase(Transform target)
    {
        chaseTarget = target;
    }

    public void Clear()
    {
        reached = false;
        going = false;
        chaseTarget = null;
    }

    public bool GoThere(Vector3 pos)
    {
        NavMeshPath path = new NavMeshPath();
        agent.CalculatePath(pos, path);
        if(path.status == NavMeshPathStatus.PathPartial
        || path.status == NavMeshPathStatus.PathInvalid)
        {
            return false;
        }
        else 
        {
            going = true;
            reached = false;
            agent.SetDestination(pos);
            return true;
        }
    }

    void Update()
    {
        if(reached) reached = false;
        if(DestinationReached())
        {
            reached = true;
            going = false;
            if(onDestinationReached != null)
            {
                onDestinationReached.Invoke();
                onDestinationReached = null;
            }
        }
        else
        {
            if(chaseTarget != null)
            {
                GoThere(chaseTarget.transform.position);
            }
        }
        if(animator) animator.SetFloat("Speed", agent.velocity.magnitude/settings.maxSpeed);
    }

    public bool GoAtPoint(int index)
    {
        if(path.points.Length == 0
        || index < 0
        || index >= path.points.Length) return false;

        if(GoThere(path.points[index]))
        {
            path.index = index;
            return true;
        }
        else return false;
    }

    public void Stop()
    {
        going = false;
        reached = false;
        chaseTarget = null;
        agent.SetDestination(transform.position);
    }

    public bool GoToNextPoint()
    {
        return GoAtPoint(GetNextPointIndex());
    }

    public int GetNextPointIndex()
    {
        int waypoint = path.index + 1;
        if(waypoint >= path.points.Length) waypoint = 0;
        return waypoint;
    }

    float DistanceToDestination()
    {
        return Vector3.Distance(transform.position, agent.destination);
    }

    bool DestinationReached()
    {
        return going && DistanceToDestination() < settings.navTreshold;
    }
}
