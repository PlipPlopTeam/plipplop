using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skeleton : MonoBehaviour
{
	[System.Serializable]
	public class Socket
	{
		public Clothes.ESlot slot;
		public Transform bone;
        public Vector3 offset;
		[HideInInspector] public Transform item;

		public bool IsFree()
		{
			return item == null;
		}

        public Vector3 GetPosition()
        {
            return bone.position;
        }

		public void Attach(Transform obj, Vector3 offset = new Vector3(), Vector3 rotate = new Vector3())
		{
            Carryable c = obj.gameObject.GetComponent<Carryable>();
		    if(c != null) c.Carry();

			obj.SetParent(bone);
			obj.transform.localPosition = offset;
            obj.forward = bone.forward;
            obj.localScale = Vector3.one;
            item = obj;
            obj.Rotate(rotate);
		}
	}
    public List<Socket> sockets = new List<Socket>();
    List<Transform> bones = new List<Transform>();

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
        return (GetSocketBySlot(Clothes.ESlot.LEFT_HAND).GetPosition() + GetSocketBySlot(Clothes.ESlot.RIGHT_HAND).GetPosition())/2f;
    }

    public void Attach(Transform t, Clothes.ESlot where, bool unequipCurrent = false, Vector3 offset = new Vector3(), Vector3 rotate = new Vector3())
    {
        Socket s = GetSocketBySlot(where);
        if(s != null)
        {
            if(!s.IsFree())
            {
                if(unequipCurrent)
                {
                    s.item.SetParent(null);
                    s.Attach(t, offset, rotate);
                }
            }
            else s.Attach(t, offset, rotate);
        }
        else Debug.LogWarning("Socket : '" + where + "' doesn't exist");
    }

    public void Drop(Clothes.ESlot from)
    {
        Socket s = GetSocketBySlot(from);
        if(s != null)
        {
            if(s.item != null)
            {
                Carryable c = s.item.gameObject.GetComponent<Carryable>();
		        if(c != null) c.Drop();
                
                s.item.SetParent(null);
                s.item = null;
            }
        }
        else Debug.LogWarning("Bone name : " + from + "doesn't exist");
    }

    public Socket GetSocketBySlot(Clothes.ESlot slot)
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
    
    void FootStep()
    {
        // For animation sound call
    }
}
