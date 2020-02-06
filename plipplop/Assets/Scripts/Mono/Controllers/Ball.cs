using UnityEngine;

public class Ball : Controller
{
	[Header("Specific properties")]
	public float jumpComboWindow = 0.5f;
	public float jumpComboHForceBonus = 1000f;
	public float jumpComboVForceBonus = 1000f;
	public int maxCombo = 5;
	public float rotationSpeed = 0.5f;
	public float velocityDamplerOnImpact = 0.75f;
	public float horizontalForce;
	public float verticalForce;

	int combo = 1;
	float comboTimer = 0f;
	public bool hopped = false;
	Vector3 lastOrientation;

	public void OnCollisionEnter(Collision collision)
	{
		if(hopped)
		{
			rigidbody.velocity *= velocityDamplerOnImpact;
			Pyromancer.PlayGameEffect("gfx_bounce", collision.GetContact(0).point);
			hopped = false;

			if (comboTimer > 0 && combo < maxCombo) combo++;
			else combo = 1;
		}
	}

	internal override void Update()
	{
		base.Update();
		if (comboTimer > 0f) comboTimer -= Time.deltaTime;
	}

	internal override void SpecificMove(Vector3 direction)
    {
		if(direction.magnitude > 0.1f)
		{
			Rotate(direction);
			if (!hopped)
			{
				Bump(direction);
				hopped = true;
			}
		}

		if(direction.magnitude > 0.25f)
		{
			lastOrientation = (Game.i.aperture.Right() * direction.x + Game.i.aperture.Forward() * direction.z);
		}
	}

	void Rotate(Vector3 direction)
	{
		Vector3 dir = (Game.i.aperture.Forward() * -direction.x + Game.i.aperture.Right() * direction.z);
		rigidbody.angularVelocity += dir * rotationSpeed;
	}

	void Bump(Vector3 direction)
	{
		Vector3 dir = (Game.i.aperture.Right() * direction.x + Game.i.aperture.Forward() * direction.z);
		rigidbody.AddForce(dir * (horizontalForce + ((combo - 1) * jumpComboHForceBonus)) * Time.deltaTime);
		rigidbody.AddForce(Vector3.up * (verticalForce + ((combo - 1) * jumpComboVForceBonus)) * Time.deltaTime);
	}

    internal override void SpecificJump()
    {
		if(comboTimer <= 0) comboTimer += jumpComboWindow;
	}

    internal override void OnLegsRetracted()
    {
		rigidbody.constraints = RigidbodyConstraints.None;
	}

    internal override void OnLegsExtended()
    {
		AlignPropOnHeadDummy();
		transform.forward = lastOrientation;
		rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
	}
}
