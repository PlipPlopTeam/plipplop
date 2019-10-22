using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PP;

[CreateAssetMenu(menuName = "Behavior/Action/Dancing")]
public class Dancing : StateActions
{
	public override void Execute(StateManager states)
	{
		states.transform.Rotate(Vector3.up * 10f);
	}
}
