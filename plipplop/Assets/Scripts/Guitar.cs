using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Guitar : Activity, ICarryable
{
	private Rigidbody rb;
	private Collider col;
	private NonPlayableCharacter player;

	public ParticleSystem music;

	public override void Enter(NonPlayableCharacter user)
	{
		base.Enter(user);
		if(player == null) BecomePlayer(user);
		else LookAtPlayer(user);
	}

	public override void Exit(NonPlayableCharacter user)
	{
		if(user == player)
		{
			music.Stop();
			user.Drop();
			transform.parent = null;
			user.animator.SetBool("Guitaring", false);
		}
		else
		{
			user.animator.SetBool("Dancing", false);
		}
		base.Exit(user);

		if(users.Count > 0)
		{
			BecomePlayer(users[0]);
			EverybodyLookAtPlayer();
		}
	}

	void LookAtPlayer(NonPlayableCharacter npc)
	{
		Vector3 pos = transform.position + Geometry.GetRandomPointAround(4f);
		npc.agentMovement.GoThere(pos);
		npc.animator.SetBool("Dancing", false);
		npc.agentMovement.onDestinationReached += () =>
		{
			npc.transform.forward = -(npc.transform.position - player.transform.position).normalized;
			npc.agentMovement.Stop();
			npc.look.FocusOn(player.transform);
			npc.animator.SetBool("Dancing", true);
		};
	}

	void EverybodyLookAtPlayer()
	{
		foreach (NonPlayableCharacter npc in users)
			if (npc != player) LookAtPlayer(npc);
	}

	void BecomePlayer(NonPlayableCharacter user)
	{
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

	void Awake()
	{
		rb = GetComponent<Rigidbody>();
		col = GetComponent<Collider>();
	}
	public virtual void Carry()
	{
		if (col != null) col.enabled = false;
		if (rb != null) rb.isKinematic = true;
	}
	public virtual void Drop()
	{
		if (col != null) col.enabled = true;
		if (rb != null) rb.isKinematic = false;
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
