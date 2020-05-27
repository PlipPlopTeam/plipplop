using UnityEngine;

public class Jumper : Controller
{
	[Header("Jumper")]
	[Header("Charge")]
	public float scaleModification = 0.25f;
	public float chargeMaxTime = 2f;
	public float chargePercentage = 0.25f;
	public float chargeMaxForce = 30000f;
	[Header("Flip")]
	public float flipUpForce = 500f;
	public float flipTorqueForce = 500f;
	[Header("Roll")]
	public float angularForce = 50f;
	public float maxVelocityMagnitude = 1f;
	[Header("Other")]
	public float breakVelocity = 2f;
	[Header("References")]
	public Transform body;
	public Chair chair;
    public int flips { get; private set; } = 0;

	private float chargeForce = 0f;
	private float chargeTime = 0f;
	private Vector3 dir = new Vector3();
	private bool charging = false;
	private bool inAir = false;

    float xAccumulatedAngle = 0f;
    float zAccumulatedAngle = 0f;
    Vector3 previousAngle;

	void Flip()
	{
		if (IsGrounded())
		{
			rigidbody.AddForce(Vector3.up * Time.deltaTime * flipUpForce);
			rigidbody.AddTorque(transform.right * Time.deltaTime * flipTorqueForce);
		}
	}

    public override void OnPossess()
    {
        base.OnPossess();
        flips = 0;
    }

    internal override void OnLegsExtended()
	{
		base.OnLegsExtended();
		inAir = false;
	}

	internal override void SpecificMove(Vector3 direction)
	{
		dir = direction;

		if(AreLegsRetracted() && !charging)
		{
			Vector3 d = (Game.i.aperture.Forward() * -dir.x + Game.i.aperture.Right() * dir.z);
			rigidbody.angularVelocity += d * angularForce;
			rigidbody.angularVelocity = Vector3.ClampMagnitude(rigidbody.angularVelocity, maxVelocityMagnitude);
		}
	}

	internal override void OnHoldJump()
	{
		base.OnHoldJump();
		if (AreLegsRetracted() && IsGrounded())
		{
			charging = true;
			if (chargeTime < chargeMaxTime) chargeTime += Time.deltaTime;

			chargeForce = (chargeTime / chargeMaxTime) * chargeMaxForce;
			float f = ForcePercentage();
			visuals.localScale = new Vector3(1f + (f * scaleModification), 1f - (f * scaleModification), 1f + (f * scaleModification));
		}
	}

	void Bump(Vector3 direction, float force)
	{
		transform.position += Vector3.up * 0.5f;
		Vector3 dir = (Game.i.aperture.Right() * direction.x + Game.i.aperture.Forward() * direction.z);
		rigidbody.AddForce(dir * force * Time.deltaTime);
		rigidbody.AddForce(Vector3.up * force * Time.deltaTime);
		inAir = true;
	}

	float ForcePercentage()
	{
		return chargeForce / chargeMaxForce;
	}

	internal override void OnReleasedJump()
	{
		base.OnReleasedJump();
		float f = ForcePercentage();

		if (f > chargePercentage) Bump(dir, chargeForce);
		else Flip();

		Initialize();
	}

	public void Initialize()
	{
		visuals.localScale = Vector3.one;
		visuals.up = transform.up;
		chargeForce = 0f;
		chargeTime = 0f;
		charging = false;

	}

	public void Break()
	{
		foreach(Transform part in body)
		{
			part.gameObject.AddComponent<Rigidbody>();
			part.gameObject.AddComponent<BoxCollider>();
			part.SetParent(null);
		}
		Kick();
		Destroy(gameObject);
	}

    internal override void FixedUpdate()
    {
        base.FixedUpdate();

        var currAngle = transform.eulerAngles.Round();

        var rightAngle = Vector3.SignedAngle(previousAngle, currAngle, Vector3.right);
        var forwardAngle = Vector3.SignedAngle(previousAngle, currAngle, Vector3.forward);
        xAccumulatedAngle += rightAngle;
        zAccumulatedAngle += forwardAngle;

        // "standing straight"
        if (IsGrounded()) {

            if (Mathf.Abs(xAccumulatedAngle) > 170f || Mathf.Abs(zAccumulatedAngle) > 170f) {
                flips++;
            }
            xAccumulatedAngle = 0f;
            zAccumulatedAngle = 0f;
        }

        previousAngle = currAngle;
    }

    internal override void Update()
	{
		base.Update();
	
		if(inAir)
		{
			if (chair != null) chair.locked = true;
			transform.up = rigidbody.velocity.normalized;
		}
		else if(charging)
		{
			if (chair != null) chair.locked = true;
			Vector3 d = (Game.i.aperture.Right() * dir.x + Game.i.aperture.Forward() * dir.z);
			visuals.up = Vector3.Lerp(visuals.up, new Vector3(d.x * 0.1f, 1f, d.z * 0.1f), Time.deltaTime * 10f);
		}
		else
		{
			if (chair != null) chair.locked = false;
		}
	}

	public void OnCollisionEnter(Collision collision)
	{
		if (rigidbody.velocity.magnitude > breakVelocity && inAir && AreLegsRetracted()) Break();
		inAir = false;
	}
}
