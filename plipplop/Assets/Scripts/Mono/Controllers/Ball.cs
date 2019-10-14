using UnityEngine;

public class Ball : Controller
{
    [Header("Specific properties")]
    public float maxSpeed = 200f;
    public float acceleration = 1000f;
    public float jumpForce = 7f;
    public float airRollFactor = 4f;
    public float moveLerp = 1;
    public float drag = 1f;

    new Renderer renderer;
    new SphereCollider collider;
    Transform childBall;

    public override void OnPossess(bool wasCrouching=false)
    {
        base.OnPossess(wasCrouching);
    }

    public override void OnEject()
    {
        base.OnEject();
    }

    internal override void SpecificMove(Vector3 direction)
    {
        Vector3 clampDirection = Vector3.ClampMagnitude(direction, 1f);
        
        var perimeter = 2 * Mathf.PI * (1f); //radius
        Vector3 _velocity = (Game.i.aperture.Right() * direction.x + Game.i.aperture.Forward() * direction.z) * Time.deltaTime;
        _velocity *= maxSpeed;
        _velocity = Vector3.ClampMagnitude(_velocity, maxSpeed);
        _velocity.y = rigidbody.velocity.y;
        rigidbody.velocity = Vector3.Lerp(rigidbody.velocity, _velocity, Time.deltaTime * moveLerp);

        transform.forward = rigidbody.velocity.normalized;

        var factor = 1;
        
        childBall.Rotate(new Vector3(
            (rigidbody.velocity.z / perimeter) * 10f,
            0f,
            - (rigidbody.velocity.x / perimeter) * 10f
        ) * factor, Space.World);
    }

    internal override void SpecificJump()
    {
        if(IsGrounded()) 
        {
            rigidbody.velocity = new Vector3(rigidbody.velocity.x, jumpForce, rigidbody.velocity.z);
        }
    }

    internal override void OnLegsRetracted()
    {
        collider.enabled = true;
        rigidbody.drag = drag;
    }

    internal override void OnLegsExtended()
    {
        collider.enabled = false;
        rigidbody.drag = baseDrag;
    }

    internal override void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        renderer = GetComponentInChildren<MeshRenderer>();
        collider = GetComponent<SphereCollider>();
        renderer.material = Instantiate<Material>(renderer.material);
        childBall = transform.GetChild(0);
        base.Start();
    }

    internal override void FixedUpdate() {}

    internal override void Update()
    {
        if (IsPossessed())
        {

        }
        base.Update();
    }
}
