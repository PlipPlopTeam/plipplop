using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Guitar : Activity, ICarryable
{
	private Rigidbody rb;
	private Collider col;
	private NonPlayableCharacter player;
	public ParticleSystem music;

	private bool carried = false;
	public bool IsCarried() { return carried; }

	public override void StartUsing(NonPlayableCharacter user)
	{
		base.StartUsing(user);
		full = true;

		player = user;
		player.Collect(this);
		player.agentMovement.onDestinationReached += () =>
		{
			player.agentMovement.Stop();
			player.animator.SetBool("Guitaring", true);
			player.animator.SetBool("Carrying", false);
			music.Play();
		};
	}

	public override void StopUsing(NonPlayableCharacter user)
	{
		base.StopUsing(user);
		user.Drop();
		user.animator.SetBool("Guitaring", false);

		full = false;
		music.Stop();
		if(spectators.Count > 0)
		{
			NonPlayableCharacter newPlayer = spectators[0];
			StopSpectate(newPlayer);
			StartUsing(newPlayer);
		}
	}

	public override void StopSpectate(NonPlayableCharacter npc)
	{
		base.StopSpectate(npc);
		npc.animator.SetBool("Dancing", false);
	}

	public override void Look(NonPlayableCharacter npc, Vector3 position)
	{
		base.Look(npc, position);
		npc.animator.SetBool("Dancing", true);
	}

	void EverybodyLookAtPlayer()
	{
		foreach (NonPlayableCharacter npc in users)
			if (npc != player) StartSpectate(npc);
	}

	void Awake()
	{
		rb = GetComponent<Rigidbody>();
		col = GetComponent<Collider>();
	}
	public virtual void Carry()
	{
		if (col != null) col.enabled = false;
		if (rb != null) rb.isKinematic = true;
		carried = true;
	}
	public virtual void Drop()
	{
		if (col != null) col.enabled = true;
		if (rb != null) rb.isKinematic = false;
		carried = false;
	}
	public float Mass()
	{
		if (rb == null) return 0;
		MeshFilter[] meshFilters = gameObject.GetComponentsInChildren<MeshFilter>();
		Vector3 size = Vector3.one;
		foreach (MeshFilter mf in meshFilters)
		{
			if (mf.mesh.bounds.size.magnitude > size.magnitude)
				size = mf.mesh.bounds.size;
		}
		return (transform.localScale.magnitude * 3f) * size.magnitude * rb.mass;
	}
	public Transform Self()
	{
		return transform;
	}
}
