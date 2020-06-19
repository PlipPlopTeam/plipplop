using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class Vanilla : Controller
{
	[Header("Settings")]
	public float angularForce = 2000f;
	public float maxVelocityMagnitude = 1f;

	internal override void OnLegsExtended() {}
    internal override void OnLegsRetracted() {}

	internal override void SpecificMove(Vector3 direction)
	{
		Vector3 d = (Game.i.aperture.Forward() * -direction.x + Game.i.aperture.Right() * direction.z);
		rigidbody.angularVelocity += d * angularForce;
		rigidbody.angularVelocity = Vector3.ClampMagnitude(rigidbody.angularVelocity, maxVelocityMagnitude);
	}

	private void OnCollisionEnter(Collision other)
	{
		if (IsPossessed() && AreLegsRetracted())
		{
			SoundPlayer.Play("sfx_guitar_impact", UnityEngine.Random.Range(.3f,.5f), UnityEngine.Random.Range(.85f,1.15f));
		}
	}
}
