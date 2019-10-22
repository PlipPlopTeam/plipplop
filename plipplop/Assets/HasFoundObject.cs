using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PP;

[CreateAssetMenu]
public class HasFoundObject : Condition
{
	public override bool CheckCondition(StateManager state)
	{
		Guard g = (Guard)state;
		if (g != null && g.found)
		{
			return true;
		}

		return false;
	}
}
