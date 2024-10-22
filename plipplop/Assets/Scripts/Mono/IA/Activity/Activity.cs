﻿using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Activity : Valuable
{
	[System.Serializable]
	public class StatMultiplier
	{
		public float boredom = 1f;
		public float hunger = 0f;
		public float tiredness = 0f;
	}

	[Header("Activity")]
	public bool activated = true;
	[HideInInspector] public bool working = true;
	public int userMax = 0;
	public int spectatorMax = 0;
	public Vector2 spectatorRange;
	public float awarnessMultiplier = 1f;
	public StatMultiplier use = new StatMultiplier();
	public StatMultiplier spectate = new StatMultiplier();
	public List<NonPlayableCharacter> users = new List<NonPlayableCharacter>();
	public List<NonPlayableCharacter> spectators = new List<NonPlayableCharacter>();
	internal float timer = 0f;
	internal bool full = false;
	
	public List<NonPlayableCharacter> all { get { return users.Union(spectators).ToList(); } }

	public virtual void Enter(NonPlayableCharacter user)
    {
		if (!full) StartUsing(user);
		else if (spectatorMax > 0) StartSpectate(user);
		else Vanish(user);

		user.movement.StopFollowingPath();
        user.stats[NonPlayableCharacter.EStat.BOREDOM] = 0f;
        user.activity = this;
		user.sight.multiplier = awarnessMultiplier;

		hidden = Used();
	}

	public void Vanish(NonPlayableCharacter user)
	{
		Exit(user);
		user.previousActivity = null;
	}

	public bool Used()
	{
		return users.Count > 0 || spectators.Count > 0;
	}

	public virtual void StartUsing(NonPlayableCharacter user)
	{
		users.Add(user);
		if (users.Count >= userMax) full = true;
	}

	public virtual void StopUsing(NonPlayableCharacter user)
	{
		users.Remove(user);
		if (users.Count < userMax) full = false;
	}

	public virtual void Exit(NonPlayableCharacter user)
    {
        user.activity = null;

		if (!user.settings.canDoActivitiesTwice) user.previousActivity = this;
		else user.previousActivity = null;

		user.sight.multiplier = 1f;

		if (users.Contains(user)) StopUsing(user);
		else if (spectators.Contains(user)) StopSpectate(user);

		hidden = Used();
	}

	public void Dismantle()
	{
		Break();
		Destroy(this, 1f);
	}

	public virtual void StartSpectate(NonPlayableCharacter npc)
	{
		spectators.Add(npc);
		Vector3 pos = transform.position + Geometry.GetRandomPointOnCircle(Random.Range(spectatorRange.x, spectatorRange.y));
		npc.movement.Stop();
		npc.movement.GoThere(pos);
		npc.movement.onDestinationReached += () =>
		{
			if(npc.activity != null)
			{
				Look(npc, transform.position);
				npc.emo.Show(Emotion.EVerb.LOVE, Name());
			}
		};
	}

	public virtual void StopSpectate(NonPlayableCharacter npc)
	{
		spectators.Remove(npc);
		npc.movement.Stop();
	}

	public virtual void Look(NonPlayableCharacter npc, Vector3 position)
	{
		npc.movement.Stop();
		npc.movement.OrientToward(transform.position);
	}

	public virtual bool AvailableFor(NonPlayableCharacter npc)
	{
		bool result = working
			&& activated
			&& !users.Contains(npc)
			&& !spectators.Contains(npc)
			&& npc.previousActivity != this;

		if(result)
		{
			if (userMax > 0) result = users.Count < userMax;
			if (spectatorMax > 0) result = spectators.Count < spectatorMax;
		}

		return result;
	}

	public virtual void KickAll()
    {
		lock (all)
		{
			foreach (NonPlayableCharacter user in all) Exit(user);
		}
	}

	public virtual void Kick(NonPlayableCharacter user)
	{
		user.emo.Show(Emotion.EVerb.HATE, Name());
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
			if (working) AffectStats();
		}
    }

	public void AffectStats()
	{
		lock(users)
		{
			foreach (NonPlayableCharacter user in users)
			{
				if(user != null)
				{
					user.AddToStat(NonPlayableCharacter.EStat.BOREDOM, use.boredom);
					user.AddToStat(NonPlayableCharacter.EStat.TIREDNESS, use.tiredness);
					user.AddToStat(NonPlayableCharacter.EStat.HUNGER, use.hunger);

					if(user.GetStat(NonPlayableCharacter.EStat.BOREDOM) > 50f
					&& user.GetStat(NonPlayableCharacter.EStat.BOREDOM) < 60f)
					{
						user.emo.Show(Emotion.EVerb.LOVE, Name());
					}
				}
			}
		}

		lock(spectators)
		{
			foreach (NonPlayableCharacter spectator in spectators)
			{
				if(spectator != null)
				{
					spectator.AddToStat(NonPlayableCharacter.EStat.BOREDOM, spectate.boredom);
					spectator.AddToStat(NonPlayableCharacter.EStat.TIREDNESS, spectate.tiredness);
					spectator.AddToStat(NonPlayableCharacter.EStat.HUNGER, spectate.hunger);
				}
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
			UnityEditor.Handles.color = new Color32(0, 255, 0, 255);
			Gizmos.DrawLine(transform.position, user.transform.position);
			UnityEditor.Handles.DrawWireDisc(user.transform.position, Vector3.up, 0.1f);
		}

		foreach (NonPlayableCharacter spectator in spectators.ToArray())
		{
			Gizmos.color = new Color32(0, 0, 255, 255);
			UnityEditor.Handles.color = new Color32(0, 0, 255, 255);
			UnityEditor.Handles.DrawWireDisc(spectator.transform.position, Vector3.up, 0.25f);
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
