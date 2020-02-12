using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class Skeleton : MonoBehaviour
{
	[System.Serializable]
	public class Socket
	{
		public Cloth.ESlot slot;
		public Transform bone;
        public Vector3 offset;
		[HideInInspector] public Transform item;

		public bool IsFree()
		{
			return item == null;
		}

        public Vector3 GetPosition()
        {
            return bone.position + bone.TransformVector(offset);
        }

		public void Attach(Transform obj, Vector3 offset = new Vector3(), Vector3 rotate = new Vector3())
		{
            ICarryable c = obj.gameObject.GetComponent<ICarryable>();
		    if(c != null) c.Carry();

			obj.SetParent(bone);
			obj.transform.localPosition = this.offset + offset;
            obj.forward = bone.forward;
			obj.localScale = Vector3.one;
            item = obj;
            obj.Rotate(rotate);
		}
	}
    public List<Socket> sockets = new List<Socket>();
    List<Transform> bones = new List<Transform>();
	public Transform hips;
	public Vector3 hairOffset;

    public void Awake()
    {
        foreach(Transform t in gameObject.GetComponentsInChildren<Transform>())
        {
            if(t != transform && !IsSocket(t)) bones.Add(t);
        }
    }

    bool IsSocket(Transform t)
    {
        foreach(Socket s in sockets)
            if(t == s.bone) return true;

        return false;
    }

    public Vector3 GetCenterOfHands()
    {
        return (GetSocketBySlot(Cloth.ESlot.LEFT_HAND).GetPosition() + GetSocketBySlot(Cloth.ESlot.RIGHT_HAND).GetPosition())/2f;
    }

    public void Attach(Transform t, Cloth.ESlot where, bool unequipCurrent = false, Vector3 offset = new Vector3(), Vector3 rotate = new Vector3())
    {
		Vector3 os = offset;

		Socket s = GetSocketBySlot(where);
        if(s != null)
        {
			// Hair Offset
			if (where == Cloth.ESlot.TOP_HEAD && !GetSocketBySlot(Cloth.ESlot.HEAD).IsFree())
			{
				os += hairOffset;
			}
			else if (where == Cloth.ESlot.HEAD && !GetSocketBySlot(Cloth.ESlot.TOP_HEAD).IsFree())
			{
				GetSocketBySlot(Cloth.ESlot.TOP_HEAD).item.localPosition += hairOffset;
			}

			if (!s.IsFree())
			{
				if (unequipCurrent)
				{
					s.item.SetParent(null);
					s.Attach(t, os, rotate);
				}
			}
			else s.Attach(t, os, rotate);
        }
        else Debug.LogWarning("Socket : '" + where + "' doesn't exist");
    }

    public void Drop(Cloth.ESlot from)
    {
		Socket s = GetSocketBySlot(from);

		if (s.slot == Cloth.ESlot.HEAD)
		{
			if (!GetSocketBySlot(Cloth.ESlot.TOP_HEAD).IsFree())
			{
				GetSocketBySlot(Cloth.ESlot.TOP_HEAD).item.localPosition -= hairOffset;
			}
		}

		if (s != null)
        {
            if(s.item != null)
            {
                ICarryable c = s.item.gameObject.GetComponent<ICarryable>();
		        if(c != null) c.Drop();
                
                s.item.SetParent(null);
                s.item = null;
            }
        }
        else Debug.LogWarning("Bone name : " + from + "doesn't exist");
    }

    public Socket GetSocketBySlot(Cloth.ESlot slot)
    {
        return sockets.Find(o => o.slot == slot);
    }

    public void Enfold (Transform t)
    {
        t.SetParent(transform);
        foreach(Transform child in t.GetComponentsInChildren<Transform>())
        {
            Transform target = GetBoneByName(child.name);
            if(target != null)
            {
                child.SetParent(target);
                child.localRotation = new Quaternion();
                child.localPosition = Vector3.zero;
            }
        }
    }

    Transform GetBoneByName(string name)
    {
        foreach(Transform b in bones)
        {
            if(b.name == name) return b;
        }
        return null;
    }

	public float GetButtHeight()
	{
		if(hips != null) return hips.localPosition.y;
		return 0f;
	}
    
    void FootStep()
    {
        // For animation sound call
    }

#if UNITY_EDITOR
	void OnDrawGizmosSelected()
	{
		foreach(Socket s in sockets)
		{
			Gizmos.DrawWireSphere(s.bone.position, 0.05f);
			Handles.Label(s.bone.position, s.bone.name);
			Gizmos.DrawLine(s.bone.position, s.GetPosition());
			Gizmos.DrawWireSphere(s.GetPosition(), 0.025f);
		}
	}
#endif
}
