using UnityEngine;

public class BirdPoint : BirdZone
{
	public override Bird.Spot GetSpot()
	{
		if (transform.childCount == 0)
		{
			return new Bird.Spot(transform, Vector3.zero);
		}
		else return null;
	}

	public override bool Available()
	{
		return transform.childCount == 0;
	}
}