using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TheReef : NonPlayableCharacter
{
	private Controller controller;
	private Controller seenPlayer;
	private NonPlayableCharacter target;

	// Throw
	public float throwForce = 10000f;
	public override void Kick(Controller c)
	{
		controller = c;
		animator.SetTrigger("Eject");
		movement.Stop();
		StartCoroutine(WaitAndKick());
	}

	public override void ShowOff(float time, Vector2 range, int slot)
	{
		base.ShowOff(time, range, slot);
		movement.RandomOrient();
		Drop();
	}

	IEnumerator WaitAndKick()
	{
		yield return new WaitForSeconds(0.5f);
		controller.Kick();
		seenPlayer = Game.i.player.GetCurrentController();
		seenPlayer.Freeze();
		seenPlayer.Paralyse();
		Drop();
		skeleton.Attach(controller.transform, Cloth.ESlot.LEFT_HAND, true);
		skeleton.Attach(seenPlayer.transform, Cloth.ESlot.RIGHT_HAND, true);
		StartCoroutine(WaitAndThrow());
	}
	IEnumerator WaitAndThrow()
	{
		yield return new WaitForSeconds(0.85f);

		if (controller.TryGetComponent(out ICarryable result)) Carry(result);
		else skeleton.Attach(controller.transform, Cloth.ESlot.RIGHT_HAND, true);
		seenPlayer.UnFreeze();
		seenPlayer.transform.position -= skeleton.GetSocketBySlot(Cloth.ESlot.RIGHT_HAND).bone.up * 2f;
		seenPlayer.Throw(-skeleton.GetSocketBySlot(Cloth.ESlot.RIGHT_HAND).bone.up, throwForce);
		seenPlayer.transform.up = Vector3.up;
		controller = null;
		seenPlayer = null;
	}
	// Reassure
	public float reassureTime = 1f;
	public void Reassure(NonPlayableCharacter npc)
	{
		target = npc;
		Drop();
		target.graph.Pause();
		target.movement.Stop();
		movement.Stop();
		Vector3 dir = (target.transform.position - transform.position).normalized;
		target.transform.forward = -dir;
		transform.forward = dir;
		StartCoroutine(WaitAndEndReassure());
	}
	IEnumerator WaitAndEndReassure()
	{
		yield return new WaitForSeconds(reassureTime);
		target.graph.Play();
		target.graph.Move(6406);
		target = null;
	}

	public override void Carrying(ICarryable carryable)
	{
		base.Carrying(carryable);
		carryable.Self().up = Vector3.up;
	}

	public override void Carry(ICarryable carryable)
	{
		if (carried != null) Drop();
		carried = carryable;
		carried.Carry();
		if (animator != null) animator.SetBool("Holding", true);
		if (skeleton != null) skeleton.Attach(carried.Self(), Cloth.ESlot.RIGHT_HAND, true);
		carried.Self().localPosition = Vector3.zero;
	}

	public override void Drop()
	{
		if (carried == null) return;
		carried.Drop();
		skeleton.Drop(Cloth.ESlot.RIGHT_HAND);
		if (animator != null)
		{
			animator.SetBool("Holding", false);
			animator.SetBool("Carrying", false);
		}
		carried = null;
	}
}
