using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TheReef : NonPlayableCharacter
{
	private Controller controller;
	private Controller player;

	public override void Kick(Controller c)
	{
		controller = c;
		controller.Kick();
		player = Game.i.player.GetCurrentController();
		skeleton.Attach(controller.transform, Clothes.ESlot.LEFT_HAND, true);
		skeleton.Attach(player.transform, Clothes.ESlot.RIGHT_HAND, true);
		animator.SetTrigger("Eject");
		StartCoroutine(WaitAndKick());
	}

	IEnumerator WaitAndKick()
	{
		yield return new WaitForSeconds(1f);
		skeleton.Drop(Clothes.ESlot.RIGHT_HAND);
		player.rigidbody.AddForce(transform.forward * 1000f * Time.deltaTime);
	}
}
