using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clothes : MonoBehaviour
{
    public enum ESlot {HEAD, FACE, NECK, TORSO, LEFT_WRIST, RIGHT_WRIST, RIGHT_HAND, LEFT_HAND, LEGS, RIGHT_FOOT, LEFT_FOOT};
    public ClothesData data;
    public Transform root;
    public List<Transform> attachs = new List<Transform>();
    public Skeleton.Socket socket;

    public virtual void Pulverize()
    {
        foreach(Transform a in attachs) Destroy(a.gameObject);
        Destroy(root.gameObject);
        if(socket != null) socket.item = null;
        Destroy(this);
    }

    public virtual void Create(ClothesData cloth, Skeleton target)
    {
        data = cloth;
        transform.SetParent(target.transform);            

        // THREADABLE
        if(data.slot == ESlot.TORSO || data.slot == ESlot.LEGS || data.slot == ESlot.RIGHT_FOOT || data.slot == ESlot.LEFT_FOOT)
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
            target.Attach(root, data.slot);
        }
    }
}

