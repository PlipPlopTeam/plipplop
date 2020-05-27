using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Chair : MonoBehaviour
{
    [System.Serializable]
    public class Spot
    {
        public Vector3 position;
        public Vector2 orientation;
        [HideInInspector] public NonPlayableCharacter user = null;
		[HideInInspector] public bool isSitted;
		[HideInInspector] public float cRotation;
    }

	[Header("Settings")]
	[Range(0f, 90f)] public float minStraightAngle = 45f;
	public Transform visual;
    public Chair.Spot[] spots;
	[HideInInspector] public bool locked;

	List<NonPlayableCharacter> users
	{
		get
		{
			List<NonPlayableCharacter> result = new List<NonPlayableCharacter>();
			foreach (Spot s in spots) if (s.user != null) result.Add(s.user);
			return result;
		}
	}

	bool isStraight;

    private int GetFreeSpot()
    {
        for(int i = 0; i < spots.Length; i++)
        {
            if(spots[i].user == null) return i;
        }
        return -1;
    }

	public void Sit(NonPlayableCharacter sitter, Spot spot)
	{
		spot.cRotation = Random.Range(spot.orientation.x, spot.orientation.y);
		Align(sitter, spot);
		spot.isSitted = true;
		if (!IsStraight()) transform.up = Vector3.up;
		if (visual != null) sitter.transform.SetParent(visual);
	}

    public void Enter(NonPlayableCharacter user)
    {
        int s = GetFreeSpot();
        if(s != -1)
        {
            spots[s].user = user;
			user.GoSitThere(this, spots[s]);
        }
    }

	bool IsStraight()
	{
		return Vector3.Angle(transform.up, Vector3.up) <= 45f;
	}

	public void MakeStraight()
	{
		transform.up = Vector3.up;
	}

	public void Align(NonPlayableCharacter sitter, Spot spot)
	{
		sitter.transform.localPosition = new Vector3(spot.position.x, spot.position.y - spot.user.skeleton.GetButtHeight(), spot.position.z);
		//sitter.movement.Orient(visual.up * spot.cRotation);
		sitter.transform.rotation = Quaternion.Euler(transform.up * spot.cRotation);
	}

	public void Update()
	{
		AlignUsers();

		if (IsStraight())
		{
			if (!isStraight) isStraight = true;
		}
		else if (isStraight)
		{
			if (!locked) Dismount();
			isStraight = false;
		}
	}

	public void AlignUsers()
	{
		foreach (Spot s in spots)
		{
			if (s.user != null && s.isSitted) Align(s.user, s);
		}
	}

	public void Dismount()
	{
		foreach (Spot s in spots)
		{
			if (s.user != null)
			{
				s.user.GetUp();
			}
		}
	}

	public void Exit(NonPlayableCharacter user)
    {
        foreach(Spot s in spots)
        {
			if (s.user == user)
			{
				s.user = null;
				s.isSitted = false;
			}
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

	public void OnDestroy()
	{
		Dismount();
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
