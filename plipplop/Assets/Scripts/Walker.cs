using UnityEngine;

public class Walker : MonoBehaviour
{
	internal Floor floor = null;

	public virtual void FixedUpdate()
	{
		Floor f = FindFloor();
		if (f != null) Enter(f);
		else if(floor != null) Leave();
	}

	// GET ALL RAYCAST HITS BELOW FEETS
	private RaycastHit[] RaycastAllToGround()
	{
		return Physics.RaycastAll(transform.position, -Vector3.up, 1f);
	}

	// FIND THE FIRST FLOOR BELOW FEETS
	private Floor FindFloor()
	{
		RaycastHit[] hits = RaycastAllToGround();
		foreach(RaycastHit h in hits)
		{
			Floor f = h.collider.gameObject.GetComponent<Floor>();
			if (f != null) return f;
		}
		return null;
	}

	public virtual void ApplyAdherence(float adherence) {}

	// ENTER INTO A NEW FLOOR
	private void Enter(Floor f)
	{
		if (floor != null) Leave();
		floor = f;
		ApplyAdherence(floor.adherence);
	}

	// LEAVE THE CURRENT FLOOR
	private void Leave()
	{
		ApplyAdherence(0f);
		floor = null;
	}

#if UNITY_EDITOR
	// Draw a gizmo if i'm being possessed
	void OnDrawGizmosSelected()
	{
		Gizmos.DrawLine(transform.position, transform.position +  - Vector3.up * 0.25f);
	}
#endif
}
