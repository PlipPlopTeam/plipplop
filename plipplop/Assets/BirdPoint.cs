using UnityEngine;

public class BirdPoint : BirdZone
{
	public override Bird.Spot GetSpot()
	{
		if (transform.childCount == 0)
		{
			return new Bird.Spot(transform, transform.position);
		}
		else return null;
	}

	public override void Enter()
	{
		base.Enter();
		full = true;
	}

	public override void Exit()
	{
		base.Exit();
		full = false;
	}

	public override bool Available()
	{
		return base.Available() && transform.childCount == 0;
	}
}