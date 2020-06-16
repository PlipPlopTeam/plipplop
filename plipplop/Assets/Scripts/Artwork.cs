using UnityEngine;

public class Artwork : Activity
{
	[Header("ARTWORK")]
	public Vector3 focusOffset;

	public void Start()
	{
		full = true;
	}

	public override void Look(NonPlayableCharacter npc, Vector3 position)
	{
		base.Look(npc, position);
		npc.look.FocusOn(transform.position + focusOffset);
	}

	public override void StopSpectate(NonPlayableCharacter npc)
	{
		npc.look.LooseFocus();
		base.StopSpectate(npc);
	}

	public override void Update()
	{
		base.Update();
		Vector3 pos = transform.position 
		+ transform.right * focusOffset.x 
		+ transform.up * focusOffset.y 
		+ transform.forward * focusOffset.z;

		foreach (var s  in spectators)
		{
			s.movement.OrientToward(pos);
			s.look.FocusOn(pos);
		}
	}


#if UNITY_EDITOR
	public override void OnDrawGizmosSelected()
	{
		base.OnDrawGizmosSelected();
		Gizmos.color = new Color32(255, 215, 0, 255);
		UnityEditor.Handles.DrawWireCube(transform.position + focusOffset, Vector3.one * 0.25f);
	}
#endif
}
