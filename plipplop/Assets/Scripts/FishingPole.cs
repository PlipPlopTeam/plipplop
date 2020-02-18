using UnityEngine;

public class FishingPole : Item
{
	public Transform poleEnd;
	public Transform poleBentEnd;
	public Transform plug;
	public LineRenderer line;
	public SkinnedMeshRenderer skinnedMeshRenderer;

	public float plugOffset;

	bool isBeingUsed;

	public void Use()
	{
		isBeingUsed = true;
		UnPlunge();
	}

	public override void Drop()
	{
		base.Drop();
		isBeingUsed = false;
		UnPlunge();
	}

	public void Plunge(Vector3 position)
	{
		line.enabled = true;
		plug.parent = null;
		plug.position = position;
		skinnedMeshRenderer.SetBlendShapeWeight(0, 100f);
	}

	public void UnPlunge()
	{
		line.enabled = false;
		plug.SetParent(transform);
		plug.localPosition = poleEnd.localPosition;
		skinnedMeshRenderer.SetBlendShapeWeight(0, 0f);
	}

	public void Update()
	{
		if(isBeingUsed)
		{
			plug.up = Vector3.up;
			plugOffset = (Mathf.Sin(Time.time) / 1000f) * 0.5f;
			plug.position = new Vector3(plug.position.x, plug.position.y + plugOffset, plug.position.z);

			Vector3 dir = (transform.position - plug.position).normalized;
			transform.right = dir;
		}
	}

	public void LateUpdate()
	{
		if (isBeingUsed)
		{
			line.SetPositions(new Vector3[] { poleBentEnd.position, plug.position });
		}
	}
}
