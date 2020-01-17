using System.Collections.Generic;
using UnityEngine;

public class Activity : MonoBehaviour
{
	[System.Serializable]
	public class StatMultiplier
	{
		public float boredom = 1f;
		public float hunger = 0f;
		public float tiredness = 0f;
	}

	[Header("Parameters")]
    public bool working = true;
	public int userMax = 1;
	public int spectatorMax = 0;
	public Vector2 spectatorRange;
	[Header("Modifiers")]
	public float awarnessMultiplier = 1f;
	public StatMultiplier use;
	public StatMultiplier spectate;

	internal List<NonPlayableCharacter> users = new List<NonPlayableCharacter>();
	internal List<NonPlayableCharacter> spectators = new List<NonPlayableCharacter>();
	internal float timer = 0f;
	internal bool full = false;

    public virtual void Enter(NonPlayableCharacter user)
    {
        user.stats[NonPlayableCharacter.EStat.BOREDOM] = 0f;
        user.activity = this;
		user.sight.multiplier = awarnessMultiplier;

		if (!full) StartUsing(user);
		else StartSpectate(user);
    }

	public virtual void StartUsing(NonPlayableCharacter user)
	{
		users.Add(user);
	}

	public virtual void StopUsing(NonPlayableCharacter user)
	{
		users.Remove(user);
	}

	public virtual void Exit(NonPlayableCharacter user)
    {
        user.activity = null;
        user.previousActivity = this;
		user.sight.multiplier = 1f;

		if (users.Contains(user)) StopUsing(user);
		else if (spectators.Contains(user)) StopSpectate(user);
	}

	public virtual void StartSpectate(NonPlayableCharacter npc)
	{
		spectators.Add(npc);
		Vector3 pos = transform.position + Geometry.GetRandomPointAround(Random.Range(spectatorRange.x, spectatorRange.y));
		npc.agentMovement.GoThere(pos);
		npc.agentMovement.onDestinationReached += () =>
		{
			Look(npc, transform.position);
		};
	}

	public virtual void StopSpectate(NonPlayableCharacter npc)
	{
		spectators.Remove(npc);
	}

	public virtual void Look(NonPlayableCharacter npc, Vector3 position)
	{
		npc.transform.forward = -(npc.transform.position - position).normalized;
		npc.agentMovement.Stop();
	}

	public virtual bool AvailableFor(NonPlayableCharacter npc)
	{
		bool result = working
			&& !users.Contains(npc)
			&& !spectators.Contains(npc)
			&& users.Count < userMax;

		if(spectatorMax > 0) result = spectators.Count < spectatorMax;

		return result;
	}

    public virtual void KickAll()
    {
        foreach(NonPlayableCharacter user in users.ToArray())
            Exit(user);
    }

    [ContextMenu("Break")]
    public virtual void Break()
    {
        KickAll();
        working = false;
    }

    [ContextMenu("Repair")]
    public virtual void Repair()
    {
        working = true;
    }

    private void Start()
    {
        Game.i.aiZone.Register(this);
    }

    public virtual void Update()
    {
        if(timer > 0f) timer -= Time.deltaTime;
        else
        {
            timer = 1f;
            foreach(NonPlayableCharacter user in users.ToArray())
            {
                user.AddToStat(NonPlayableCharacter.EStat.BOREDOM, use.boredom);
                user.AddToStat(NonPlayableCharacter.EStat.TIREDNESS, use.tiredness);
                user.AddToStat(NonPlayableCharacter.EStat.HUNGER, use.hunger);
            }

			foreach (NonPlayableCharacter spectator in spectators.ToArray())
			{
				spectator.AddToStat(NonPlayableCharacter.EStat.BOREDOM, spectate.boredom);
				spectator.AddToStat(NonPlayableCharacter.EStat.TIREDNESS, spectate.tiredness);
				spectator.AddToStat(NonPlayableCharacter.EStat.HUNGER, spectate.hunger);
			}
		}
    }


#if UNITY_EDITOR
	public virtual void OnDrawGizmosSelected()
	{
		if (spectatorMax > 0)
		{
			UnityEditor.Handles.color = new Color32(0, 0, 255, 255);
			UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.up, spectatorRange.x);
			UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.up, spectatorRange.y);
		}

		foreach (NonPlayableCharacter user in users.ToArray())
		{
			Gizmos.color = new Color32(0, 255, 0, 255);
			Gizmos.DrawLine(transform.position, user.transform.position);
		}

		foreach (NonPlayableCharacter spectator in spectators.ToArray())
		{
			Gizmos.color = new Color32(0, 0, 255, 255);
			Gizmos.DrawLine(transform.position, spectator.transform.position);
		}
	}

	void OnValidate()
	{
		if(spectatorMax > 0)
		{
			if (spectatorRange.x < 0) spectatorRange.x = 0;
			if (spectatorRange.y < spectatorRange.x) spectatorRange.y = spectatorRange.x;
		}
	}
#endif
}
