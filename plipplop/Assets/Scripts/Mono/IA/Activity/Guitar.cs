using UnityEngine;
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
		player.agentMovement.onDestinationReached += () =>
		{
			player.agentMovement.Stop();
			player.animator.SetBool("Guitaring", true);
			player.animator.SetBool("Carrying", false);

			Game.i.WaitAndDo(1f, () =>
			{
				visuals.transform.localPosition = new Vector3(0.114f, -0.08f, -0.155f);
				visuals.transform.localEulerAngles = new Vector3(3.809f, 5.44f, 55.098f);
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
		user.Drop();
		user.StopCollecting();

		player.agentMovement.onDestinationReached -= () =>
		{
			player.agentMovement.Stop();
			player.animator.SetBool("Guitaring", true);
			player.animator.SetBool("Carrying", false);
			visuals.transform.localPosition = new Vector3(0.114f, -0.08f, -0.155f);
			visuals.transform.localEulerAngles = new Vector3(3.809f, 5.44f, 55.098f);
			PlayMusic();
		};

		user.animator.SetBool("Guitaring", false);
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
