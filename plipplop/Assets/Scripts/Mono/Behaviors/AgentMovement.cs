using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

#if UNITY_EDITOR
using UnityEditor;
#endif

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
        public float minimumCarrySpeed = 1f;  
    }

    public System.Action onDestinationReached;
    public System.Action onPathCompleted;
    public System.Action onTargetOffPath;
    public AgentMovement.Path path;
    public AgentMovement.Settings settings;

    [HideInInspector] public bool going = false;
    [HideInInspector] public bool reached = false;
    [HideInInspector] public Animator animator;

    private bool followingPath;
    private Transform chaseTarget;
    private NavMeshAgent agent;

    public void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        going = false;
        SetSpeed(settings.speed);
    }

    public void FollowPath(AgentMovement.Path newPath)
    {
        path = newPath;
        followingPath = true;
        onPathCompleted += () => {if(!path.loop) followingPath = false;};
        GoAtPoint(0);
    }

    public void ApplyWeightToSpeed(float weight, float strength)
    {
        float s = (settings.speed - settings.minimumCarrySpeed) * Mathf.Max(strength/weight) + settings.minimumCarrySpeed;
        SetSpeed(s);
    }

    public void ResetSpeed()
    {
        SetSpeed(settings.speed);
    }

    public void SetSpeed(float value)
    {
        if(value < 0) value = 0f;
        agent.speed = value;
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

            if(path.index == path.points.Length - 1)
            {
                if(onPathCompleted != null)
                {
                    onPathCompleted.Invoke();
                    onPathCompleted = null;
                }
            }

            if(followingPath) GoToNextPoint();
        }
        else
        {
            if(chaseTarget != null)
            {
                if(!GoThere(chaseTarget.transform.position))
                {
                    if(onTargetOffPath != null)
                    {
                        onTargetOffPath.Invoke();
                        onTargetOffPath = null;
                    }
                }
            }
        }

        if(animator) 
        {
            animator.SetFloat("Speed", agent.velocity.magnitude/settings.maxSpeed);
        }
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

    public void StopChase()
    {
        chaseTarget = null;
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

    
#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        Gizmos.color = new Color32(255, 215, 0, 255);
        if(EditorApplication.isPlaying && agent != null && agent.destination != null)
        {
            UnityEditor.Handles.DrawLine(transform.position, agent.destination);
        }
    }
#endif
}
