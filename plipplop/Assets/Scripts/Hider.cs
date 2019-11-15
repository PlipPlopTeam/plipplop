using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class Hider : MonoBehaviour
{
    [Header("Hider Settings")]
    public float range = 1f;
    public bool activated = true;

    SphereCollider sc;
    void Start()
    {
        if(sc == null) sc = gameObject.GetComponent<SphereCollider>();
        SetRange(range);

        if(activated) Activate();
        else Desactivate();
    }

    public virtual void SetRange(float value)
    {   
        range = value;
    }

    [ContextMenu("Activate")]
    public virtual void Activate()
    {
        activated = true;
    }
    [ContextMenu("Desactivate")]
    public virtual void Desactivate()
    {
        activated = false;
    }

    public virtual void Enter(Noticeable n)
    {
        n.SetVisible(false);
    }

    public virtual void Exit(Noticeable n)
    {
        n.SetVisible(true);
    }

    void OnTriggerEnter(Collider other)
    {
        Noticeable n = other.GetComponent<Noticeable>();
        if(n != null) Enter(n);
    }
    void OnTriggerExit(Collider other)
    {
        Noticeable n = other.GetComponent<Noticeable>();
        if(n != null) Exit(n);
    }

    void OnValidate()
    {
        if(sc == null) sc = gameObject.GetComponent<SphereCollider>();
        SetRange(range);
        sc.isTrigger = true;
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color32(255, 215, 0, 255);
        UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.up, range);
    }
#endif
}
