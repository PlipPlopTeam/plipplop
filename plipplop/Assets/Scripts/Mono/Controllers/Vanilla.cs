using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
}
