using UnityEngine;

public class Jumper : Controller
{
	[Header("Jumper")]
	public float scaleModification = 0.25f;
	public float flipUpForce = 500f;
	public float flipTorqueForce = 500f;
	public float chargeMaxTime = 2f;
	public float chargeMaxForce = 30000f;

	public float angularForce = 50f;
	public float maxVelocityMagnitude = 1f;

	private float chargeForce = 0f;
	private float chargeTime = 0f;
	private Vector3 dir = new Vector3();
	private bool charging = false;
	private bool inAir = false;

	void Flip()
	{
		if (IsGrounded())
		{
			rigidbody.AddForce(Vector3.up * Time.deltaTime * flipUpForce);
			rigidbody.AddTorque(transform.right * Time.deltaTime * flipTorqueForce);
		}
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
		if (IsGrounded())
		{
			charging = true;
			if (chargeTime < chargeMaxTime) chargeTime += Time.deltaTime;

			chargeForce = (chargeTime / chargeMaxTime) * chargeMaxForce;
			float f = ForcePercentage();
			visuals.localScale = new Vector3(1f + (f * scaleModification), 1f - (f * scaleModification), 1f + (f * scaleModification));
			Debug.Log(f);
		}
	}

	void Bump(Vector3 direction, float force)
	{
		Vector3 dir = (Game.i.aperture.Right() * direction.x + Game.i.aperture.Forward() * direction.z);
		//rigidbody.AddTorque(dir * force * Time.deltaTime);
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

		if (f > 0.1f) Bump(dir, chargeForce);
		else Flip();

		Initialize();
	}

	public void Initialize()
	{
		visuals.localScale = Vector3.one;
		chargeForce = 0f;
		chargeTime = 0f;
		charging = false;
	}

	internal override void Update()
	{
		base.Update();
	
		if(inAir)
		{
			transform.up = rigidbody.velocity.normalized;
		}
	}

	public void OnCollisionEnter(Collision collision)
	{
		inAir = false;
	}
}
