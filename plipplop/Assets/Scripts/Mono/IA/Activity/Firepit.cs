using UnityEngine;

public class Firepit : Activity
{
    [Header("Firepit")]
	public Vector2 radius;

	public override void Enter(NonPlayableCharacter user)
    {
        base.Enter(user);
		Chair chair = GetChairInRange();

		foreach (NonPlayableCharacter u in users)
        {
            NonPlayableCharacter npc = users[Random.Range(0, users.Count)];
            if(npc != u) u.look.FocusOn(npc.skeleton.GetSocketBySlot(Cloth.ESlot.HEAD).bone);
        }

        if(chair == null)
        {
            Vector3 pos = Geometry.GetRandomPointOnCircle(Random.Range(radius.x, radius.y)) + transform.position;

            user.GoSitThere(pos);
            user.agentMovement.onDestinationReached += () =>
            {
                user.transform.LookAt(transform.position);
            };
        }
        else chair.Enter(user);
    }

	Chair GetChairInRange()
	{
		RaycastHit[] hits = Physics.SphereCastAll(transform.position, radius.y, Vector3.right);
		foreach (RaycastHit h in hits)
		{
			Chair c = h.collider.gameObject.GetComponent<Chair>();
			if (c!= null && !c.IsFull()) return c;
		}
		return null;
	}

    public override void Exit(NonPlayableCharacter user)
    {
        user.GetUp();
        user.look.LooseFocus();
        base.Exit(user);
    }

#if UNITY_EDITOR
	public override void OnDrawGizmosSelected()
	{
		base.OnDrawGizmosSelected();
		Gizmos.color = new Color32(255, 215, 0, 255);
        UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.up, radius.x);
        UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.up, radius.y);
	}

	void OnValidate()
	{
		if (radius.x < 0) radius.x = 0;
		if (radius.y < radius.x) radius.y = radius.x;
	}
#endif
}
