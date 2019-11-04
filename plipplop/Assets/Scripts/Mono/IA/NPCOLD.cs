using System.Collections.Generic;
using System.Collections;
using UnityEngine.AI;
using UnityEngine;

public enum ActionState{ Walking, Collecting, Sort }

public class NPCOLD : MonoBehaviour
{
    
    [Header("State")]
    public ActionState state;

    [Header("Movement")]
    public float walkSpeed = 5f;
    public float chaseSpeed = 8f;
    public float maxSpeed = 1f;
    public float velocityLerpSpeed = 1f;
    public float navTreshold = 1f;
    
    public bool loop = true;
    public Vector3[] path;
    int point = 0;

    Sight sight;
    FocusLook look;
    NavMeshAgent agent;
    Animator animator;
    Valuable valuable;
    Skeleton skeleton;
    EmotionRenderer emo;
    
    // Range
    CollisionEventTransmitter range;
    List<GameObject> inRange = new List<GameObject>();

    void Awake()
    {
        skeleton = GetComponentInChildren<Skeleton>();
        sight = GetComponent<Sight>();
        look = GetComponent<FocusLook>();
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        emo = GetComponent<EmotionRenderer>();

        range = GetComponentInChildren<CollisionEventTransmitter>();
        range.onTriggerEnter += (Collider other) => { inRange.Add(other.transform.gameObject); };
        range.onTriggerExit += (Collider other) => { inRange.Remove(other.transform.gameObject); };
    }

    void Start()
    {
        Go(point);
    }

    void ChangeState(ActionState newState)
    {
        ExitState();
        state = newState;
        switch(state)
        {
            case ActionState.Walking:
                agent.speed = walkSpeed;
                break;
            case ActionState.Collecting:
                agent.speed = chaseSpeed;
                break;
            case ActionState.Sort:
                if(!GoThere(valuable.origin))
                {
                    valuable.transform.position = transform.position + transform.forward;
                    valuable = null;
                    ChangeState(ActionState.Walking);
                }
                else
                {
                    agent.speed = walkSpeed;
                    animator.SetBool("Carrying", true); 
                }
                break;
        }
    }

    bool InRange(GameObject obj)
    {
        return inRange.Contains(obj);
    }

    void ExitState()
    {
        switch(state)
        {
            case ActionState.Walking: 
                break;
            case ActionState.Collecting: 
                break;
            case ActionState.Sort:
                animator.SetBool("Carrying", false);
                break;
        }
    }

    void StateUpdate()
    {
        switch(state)
        {
            case ActionState.Walking:
                Search();
                break;
            case ActionState.Collecting:
                agent.destination = valuable.transform.position;
                if(InRange(valuable.gameObject))
                {
                    emo.Show("suprised", 2f);
                    Controller c = valuable.gameObject.GetComponent<Controller>();
                    if(c != null)
                    {
                        if(Game.i.player.IsPossessing(c))
                            Game.i.player.TeleportBaseControllerAndPossess();
                    }
                    ChangeState(ActionState.Sort);
                }
                break;
            case ActionState.Sort:
                valuable.transform.position = (skeleton.rightHandBone.position + skeleton.leftHandBone.position)/2f;
                valuable.transform.forward = transform.forward;
                break;
        }
    }

    bool GoThere(Vector3 pos)
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
            agent.SetDestination(pos);
            return true;
        }
    }

    void DestinationReached()
    {
        switch(state)
        {
            case ActionState.Walking:
                Go(Next());
                break;
            case ActionState.Collecting: 
                break;
            case ActionState.Sort:
                valuable.transform.position = valuable.origin;
                valuable = null;
                ChangeState(ActionState.Walking);
                break;
        }
    }

    void Go(int index)
    {
        if(path.Length == 0) return;
        agent.SetDestination(path[point]);
        point = index;
    }

    int Next()
    {
        int waypoint = point + 1;
        if(waypoint >= path.Length) waypoint = 0;
        return waypoint;
    }

    void Collect(Valuable v)
    {
        valuable = v;
        ChangeState(ActionState.Collecting);
    }

    void Update()
    {
        StateUpdate();
        animator.SetFloat("Speed", agent.velocity.magnitude/maxSpeed);
        if(Vector3.Distance(transform.position, agent.destination) < navTreshold) DestinationReached();
    }



    void Search()
    {
        Valuable[] seens = sight.Scan<Valuable>();
        if(seens.Length > 0)
        {
            if(seens[0].HasMoved() && !seens[0].hidden)
            {
                animator.SetTrigger("Suprised");
                emo.Show("confused", 2f);
                Collect(seens[0]);
            }
        }
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        GUIStyle style = new GUIStyle();
        style.normal.textColor = new Color32(173, 216, 230, 255);
        style.alignment = TextAnchor.MiddleCenter;
        style.fontStyle = FontStyle.Bold;
        style.fontSize = 9;
        if(path != null && path.Length > 0)
        {
            Gizmos.DrawLine(transform.position, path[0]);
            Gizmos.DrawIcon(path[0] + transform.up * 0.5f, "d_CollabMoved Icon");
            UnityEditor.Handles.Label(path[0] + transform.up, "0", style);
            
            if(path.Length > 1)
            {
                for(int i = 1; i < path.Length; i++)
                {
                    Gizmos.DrawLine(path[i-1], path[i]);
                    Gizmos.DrawIcon(path[i] + transform.up * 0.5f, "d_CollabMoved Icon");
                    UnityEditor.Handles.Label(path[i] + transform.up, i.ToString(), style);
                }
            }
        }
    }
#endif
}
