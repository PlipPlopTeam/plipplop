using UnityEngine;

public class GuitarController : Controller
{
	internal override void OnLegsExtended() { }
	internal override void OnLegsRetracted() { }

	[Header("Guitar Controller")]
	[Header("References")]
	public CollisionEventTransmitter impactDetection;
	public ParticleSystem noteBurstParticle;
	public Activity linkedActivity;
	[Header("Settings")]
	public float angularForce = 50f;
	public float breakMagnitude = 5f;
	public float maxVelocityMagnitude = 1f;
	public bool alsoBreakActivity = false;

	private bool broken;

	public override void OnEject()
	{
		base.OnEject();
		locomotion.locomotionAnimation.legs.stepGFX = "";
	}

	public override void OnPossess()
	{
		base.OnPossess();
		if (broken) locomotion.locomotionAnimation.legs.stepGFX = "gfx_guitar_bad_chord";
		else locomotion.locomotionAnimation.legs.stepGFX = "gfx_guitar_good_chord";
	}

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

	private void OnCollisionEnter(Collision collision)
	{
		if(rigidbody != null && rigidbody.velocity.magnitude > breakMagnitude)
		{
			Break();
		}
		else
		{
			if(collision.gameObject.TryGetComponent<Rigidbody>(out Rigidbody otherRb))
			{
				if (otherRb.velocity.magnitude > breakMagnitude)
				{
					Break();
				}
			}
		}
	}

	public void Break()
	{
		SoundPlayer.PlayAtPosition("sfx_guitar_break", transform.position);
		locomotion.locomotionAnimation.legs.stepGFX = "gfx_guitar_bad_chord";

		if (linkedActivity != null && alsoBreakActivity) linkedActivity.Break();
		broken = true;
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
