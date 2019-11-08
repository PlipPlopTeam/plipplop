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

		public void Attach(Transform obj, Vector3 offset = new Vector3())
		{
			obj.SetParent(bone);
			obj.transform.localPosition = offset;
		}
	}

    public void Attach(Transform t, string where, bool unequipCurrent)
    {
        Socket s = GetSlotByName(where);
        if(s != null)
        {
            if(!s.IsFree())
            {
                if(unequipCurrent)
                {
                    s.item.SetParent(null);
                    Link(t, s);
                }
            }
            else Link(t, s);
        }
        else Debug.LogWarning("Bone name : " + where + "doesn't exist");
    }

    private void Link(Transform t, Socket socket)
    {
        t.SetParent(socket.bone);
        t.transform.localPosition = socket.offset;
        t.forward = socket.bone.forward;
        t.localScale = Vector3.one;
        socket.item = t;
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
