﻿using UnityEngine;
using System.Collections.Generic;

public class Guitar : Activity, ICarryable
{
	[Header("Guitar")]
	public ParticleSystem musicParticle;
	public List<string> musicNames = new List<string>();
	private string currentMusicName = "";
	private NonPlayableCharacter player;

	public override void StartUsing(NonPlayableCharacter user)
	{
		base.StartUsing(user);
		full = true;
		player = user;
		player.Collect(this);
		player.movement.onDestinationReached += () =>
		{
			user.Drop();
			player.movement.Stop();
			player.animator.SetBool("Guitaring", true);
			player.animator.SetBool("Carrying", false);

			visuals.transform.parent = null;
			visuals.transform.SetParent(player.transform);
			rb.isKinematic = true;
			
			visuals.transform.localPosition = new Vector3(-0.001f, 0.987f, 0.186f);
			visuals.transform.localEulerAngles =  new Vector3(-5.062f, 10.575f, 57.788f);
			
			Game.i.WaitAndDo(.5f, () =>
			{
				PlayMusic();
			});
		};
	}

	void PlayMusic()
	{
		musicParticle.Play();

		if (musicNames.Count == 0) return;
		currentMusicName = musicNames.PickRandom();
		SoundPlayer.PlaySoundAttached(currentMusicName, transform, 0.25f, false, true);
	}

	void StopMusic()
	{
		musicParticle.Stop();
		if (currentMusicName == "") return;
		SoundPlayer.StopSounds(currentMusicName, true);
		currentMusicName = "";
	}

	public override void StopUsing(NonPlayableCharacter user)
	{
		base.StopUsing(user);
		
		user.StopCollecting();

		player.movement.onDestinationReached -= () =>
		{
			user.Drop();
			player.movement.Stop();
			player.animator.SetBool("Guitaring", true);
			player.animator.SetBool("Carrying", false);
			visuals.transform.parent = null;
			visuals.transform.SetParent(player.transform);
			rb.isKinematic = true;
			visuals.transform.localPosition = new Vector3(-0.001f, 0.987f, 0.186f);
			visuals.transform.localEulerAngles =  new Vector3(-5.062f, 10.575f, 57.788f);
			PlayMusic();
		};

		user.animator.SetBool("Guitaring", false);
		rb.isKinematic=false;

		visuals.transform.SetParent(transform);
		visuals.transform.localPosition = Vector3.zero;
		visuals.transform.localEulerAngles = Vector3.zero;
		full = false;
		if (spectators.Count > 0)
		{
			NonPlayableCharacter newPlayer = spectators[0];
			StopSpectate(newPlayer);
			StartUsing(newPlayer);
		}
		StopMusic();
	}
	
	public override void StopSpectate(NonPlayableCharacter npc)
	{
		base.StopSpectate(npc);
		npc.animator.SetBool("Dancing", false);
	}

	public override void Look(NonPlayableCharacter npc, Vector3 position)
	{
		base.Look(npc, position);
		npc.animator.SetBool("Dancing", true);
	}
}
