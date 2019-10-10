using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public abstract class Controller : MonoBehaviour
{
    [Header("Inherited properties")]
    public bool addRigidBody = false;
    public bool autoPossess = false;

    public abstract void OnEject();
    public abstract void OnPossess();
    public abstract void OnJump();
    public abstract void OnToggleCrouch(bool crouching);

    public abstract void Move(Vector3 direction);
    public void Move(float fb, float rl) {
        Move(new Vector3(rl, 0f, fb));
    }

    internal bool IsPossessed()
    {
        return Game.i.player.IsPossessing(this);
    }

    virtual internal void Awake()
    {
        if (addRigidBody)
            gameObject.AddComponent<Rigidbody>();
    }

    virtual internal void Start()
    {
        if (autoPossess)
            Game.i.player.Possess(this);
    }

    virtual internal void Update()
    {

    }


    virtual internal void FixedUpdate()
    {

    }

    // Draw a gizmo if i'm being possessed
    void OnDrawGizmos()
    {
        if (EditorApplication.isPlaying) {
            if (IsPossessed()) {
                Gizmos.DrawIcon(transform.position + Vector3.up * 2f, "Favorite Icon");
            }
            else {
                Gizmos.DrawIcon(transform.position + Vector3.up * 2f, "d_CollabChangesConflict Icon");
            }
        }
    }
}
