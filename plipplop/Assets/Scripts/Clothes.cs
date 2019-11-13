using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clothes : MonoBehaviour
{
    public enum Slot {Head, Face, Neck, Torso, LeftWrist, RightWrist, RightHand, LeftHand, Legs, RightFeet, LeftFeet};
    public ClothesData data;
    public Transform root;
    public List<Transform> attachs = new List<Transform>();
    public Skeleton.Socket socket;

    public virtual void Pulverize()
    {
        foreach(Transform a in attachs) Destroy(a.gameObject);
        Destroy(root);
        if(socket != null) socket.item = null;
        Destroy(this);
    }

    public virtual void Create(ClothesData cloth, Skeleton target)
    {
        data = cloth;
        transform.SetParent(target.transform);            

        // THREADABLE
        if(data.slot == Slot.Torso || data.slot == Slot.Legs || data.slot == Slot.RightFeet || data.slot == Slot.LeftFeet)
        {
            root = Instantiate(data.prefab, transform).transform;
            foreach(Transform child in root.GetComponentsInChildren<Transform>())
                if(child != root) attachs.Add(child);
            target.Enfold(root);
        }
        // PAIRED
        else
        {
            root = Instantiate(data.prefab, transform).transform;
            target.Attach(root, data.slot.ToString());
        }
    }
}

