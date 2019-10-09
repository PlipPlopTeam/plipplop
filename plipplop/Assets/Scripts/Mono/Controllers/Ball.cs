using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : Controller
{
    [Header("Specific properties")]
    public float maxSpeed = 1000f;
    public float acceleration = 100f;

    Renderer renderer;

    public override void Move(Vector3 direction)
    {
        throw new System.NotImplementedException();
    }

    public override void OnPossess()
    {
        renderer.material.color = Color.red;
    }

    public override void OnEject()
    {
        renderer.material.color = Color.white;
    }

    public override void OnJump()
    {
        throw new System.NotImplementedException();
    }

    public override void OnToggleCrouch(bool crouching)
    {
        throw new System.NotImplementedException();
    }

    private void Start()
    {
        renderer = GetComponentInChildren<MeshRenderer>();
        renderer.material = Instantiate<Material>(renderer.material);
    }

    private void Update()
    {
        if (IsPossessed()) {

        }
    }
}
