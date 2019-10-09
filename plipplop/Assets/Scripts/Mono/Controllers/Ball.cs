using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : Controller
{
    [Header("Specific properties")]
    public float maxSpeed = 1000f;

    new Rigidbody rigidbody;
    Renderer renderer;
    GameObject childBall;

    public override void Move(Vector3 direction)
    {
        rigidbody.AddForce(transform.forward * direction.z * maxSpeed * Time.deltaTime);
        rigidbody.AddForce(transform.right * direction.x * maxSpeed * Time.deltaTime);

        //throw new System.NotImplementedException();
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
        rigidbody = GetComponent<Rigidbody>();
        renderer = GetComponentInChildren<MeshRenderer>();
        renderer.material = Instantiate<Material>(renderer.material);
        childBall = transform.GetChild(0).gameObject;
    }

    private void Update()
    {
        if (IsPossessed()) {

        }
    }
}
