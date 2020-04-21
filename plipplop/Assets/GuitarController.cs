using UnityEngine;

public class GuitarController : Controller
{
	internal override void OnLegsExtended() { }
	internal override void OnLegsRetracted() { }

	[Header("Guitar Controller")]
	[Header("References")]
	public CollisionEventTransmitter impactDetection;
	public ParticleSystem noteBurstParticle;
	[Header("Settings")]
	public float angularForce = 50f;
	public float maxVelocityMagnitude = 1f;

	private bool broken;

	internal override void Start()
	{
		base.Start();
		impactDetection.onTriggerEnter += (other) =>
		{
			if(!transform.IsYourselfCheck(other.transform) && movingStick)
			{
				Pyromancer.PlayGameEffect("gfx_guitar_impact", transform.position);
			}
		};
	}

	internal override void Shout()
	{
		if(broken)
		{
			Pyromancer.PlayGameEffect("gfx_guitar_bad_chord", transform.position);
		}
		else
		{
			Pyromancer.PlayGameEffect("gfx_guitar_good_chord", transform.position);
		}
		noteBurstParticle.Play();
	}

	internal override void SpecificMove(Vector3 direction)
	{
		Vector3 dir = (Game.i.aperture.Forward() * -direction.x + Game.i.aperture.Right() * direction.z);
		rigidbody.angularVelocity += dir * angularForce;
		rigidbody.angularVelocity = Vector3.ClampMagnitude(rigidbody.angularVelocity, maxVelocityMagnitude);
	}
}
