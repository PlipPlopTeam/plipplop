using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skeleton : MonoBehaviour
{
	[System.Serializable]
	public class Socket
	{
		public string name;
		public Transform bone;
        public Vector3 offset;
		[HideInInspector] public Transform item;

		public bool IsFree()
		{
			return item == null;
		}

		public void Attach(Transform obj, Vector3 offset = new Vector3(), Vector3 rotate = new Vector3())
		{
			obj.SetParent(bone);
			obj.transform.localPosition = offset;
            obj.forward = bone.forward;
            obj.localScale = Vector3.one;
            item = obj;
            obj.Rotate(rotate);
		}
	}
    public void Attach(Transform t, string where, bool unequipCurrent, Vector3 offset = new Vector3(), Vector3 rotate = new Vector3())
    {
        Socket s = GetSlotByName(where);
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
        else Debug.LogWarning("Bone name : " + where + "doesn't exist");
    }

    public void Drop(string from)
    {
        Socket s = GetSlotByName(from);
        if(s != null)
        {
            if(s.item != null)
            {
                s.item.SetParent(null);
                s.item = null;
            }
        }
        else Debug.LogWarning("Bone name : " + from + "doesn't exist");
    }

    public Socket GetSlotByName(string name)
    {
        foreach(Socket s in slots) if(s.name == name) return s;
        return null;
    }

    [Header("Bones")]
    public Transform rightHandBone;
    public Transform leftHandBone;
    public Transform headBone;
    [Header("Attachs")]
    public Socket[] slots;
    
    void FootStep()
    {
        // For animation sound call
    }
}
