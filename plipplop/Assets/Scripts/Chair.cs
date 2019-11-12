using UnityEngine;

public class Chair : MonoBehaviour
{
    [System.Serializable]
    public class Spot
    {
        public Vector3 position;
        public Vector2 orientation;
        [HideInInspector] public NonPlayableCharacter user = null;
    }

    public Chair.Spot[] spots;

    private int GetFreeSpot()
    {
        for(int i = 0; i < spots.Length; i++)
        {
            if(spots[i].user == null) return i;
        }
        return -1;
    }

    public void Enter(NonPlayableCharacter user)
    {
        int s = GetFreeSpot();
        spots[s].user = user;
        user.agentMovement.GoThere(transform.position + spots[s].position);
        user.agentMovement.onDestinationReached += () =>
        {
            user.Sit(this, spots[s].position);
            user.transform.forward = transform.forward;
            user.transform.Rotate(user.transform.up * Random.Range(spots[s].orientation.x, spots[s].orientation.y));
        };
    }

    public void Exit(NonPlayableCharacter user)
    {
        user.GetUp();
        foreach(Spot s in spots)
        {
            if(s.user == user) s.user = null;
        }
    }

    public bool IsFull()
    {
        foreach(Spot s in spots)
        {
            if(s.user == null) return false;
        }
        return true;
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color32(255, 215, 0, 255);

        foreach(Spot s in spots)
        {
            UnityEditor.Handles.DrawWireDisc(transform.TransformPoint(s.position), transform.up, 0.25f);

            

            UnityEditor.Handles.DrawWireArc(
                transform.TransformPoint(s.position),
                transform.up,
                transform.forward,
                s.orientation.y,
                1f
            );

            UnityEditor.Handles.DrawWireArc(
                transform.TransformPoint(s.position),
                transform.up,
                transform.forward,
                s.orientation.x,
                1f
            );
        }
    }
#endif
}
