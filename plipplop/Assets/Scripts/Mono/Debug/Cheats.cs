using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.SceneManagement;

public class Cheats : Dictionary<string, System.Action> 
{
    public Cheats()
    {
        Add("RESET", ResetScene);
        Add("ZIK", delegate { SoundPlayer.Play("bgm_test"); });
        Add("GLUE", delegate { Game.i.player.Paralyze(); });
        Add("FREE", delegate { Game.i.player.Deparalyze(); });
        Add("FAMINE", SetAllHunderToHundred);
        Add("BIGHEADS", BigHeads);
        Add("LONGHEADS", LongHeads);
        Add("MAUVAISESTETES", MauvaisesTetes);
        Add("HELLO", SpawnNPC);
        Add("IMONFIRE", SpawnFire);
        Add("STEP", SpawnPoof);
		Add("BOSS", Bossfight);
		Add("MOUETTE", Mouette);
		Add("CRAB", Crab);
		Add("RAYBAN", Rayban);
        Add("SHAKEY", DebugShake);
        Add("BRRRRR", Rumble);
        Add("BIRDS", Hitchcock);
        Add("GRIFFIN", Griffin);
        Add("BUBBLE", DisplayDefaultBubble);
        Add("POSTPROCESS", PostProcess);
        Add("CHUT", SkipTutorial);
        Add("NOINPUTS", ToggleInputs);
    }

    public void SkipTutorial()
    {
        var assist = Object.FindObjectOfType<SpielbergAssistant>();
        if (assist != null) {
            assist.ShutDownCinematic();
        }
        var dial = Object.FindObjectOfType<DialogPlayer>();
        if (dial != null) {
            dial.EndDialogue();
        }

        var mmquest = Object.FindObjectOfType<MwonMwonIntroQuest>();
        if (mmquest != null) {
            mmquest.stopGap.SetActive(false);
        }
    }

    public void DisplayDefaultBubble()
    {
        Object.FindObjectOfType<NonPlayableCharacter>().emo.Show(Emotion.EVerb.LOVE, "beach_ball", Emotion.EBubble.THINK);
    }


    public void Hitchcock()
	{
		foreach (Bird b in Object.FindObjectsOfType<Bird>())
		{
			b.GoSitOnSpot();
		}
	}

	public void Griffin()
	{
		foreach (Bird b in Object.FindObjectsOfType<Bird>())
		{
			b.visuals.localScale = Vector3.one * Random.Range(5f, 10f);
		}
	}

	public void Rumble()
    {
        Game.i.player.Rumble(0.1f, 1f);
    }

    public void DebugShake()
    {
        Game.i.aperture.Shake(0.2f, 0.2f, 1f);
    }

	public void BigHeads()
	{
		foreach(NonPlayableCharacter npc in Object.FindObjectsOfType<NonPlayableCharacter>())
		{
			npc.skeleton.GetSocketBySlot(Cloth.ESlot.HEAD).bone.localScale = Vector3.one * Random.Range(3f, 6f);
		}
	}

	public void Rayban()
	{
		foreach (NonPlayableCharacter npc in Object.FindObjectsOfType<NonPlayableCharacter>())
		{
			npc.Equip(Game.i.library.headClothes[3], true);
		}
	}

	public void Bossfight()
	{
		foreach (NonPlayableCharacter npc in Object.FindObjectsOfType<NonPlayableCharacter>())
		{
			ICarryable torch = Game.Instantiate(Game.i.library.torchPrefab).GetComponent<ICarryable>();
			npc.movement.Chase(Game.i.player.GetCurrentController().transform);
			npc.Carry(torch);
			npc.sight.multiplier = 0f;
		}
		Game.i.aperture.currentCamera.GetComponent<UnityEngine.Experimental.Rendering.HDPipeline.HDAdditionalCameraData>().backgroundColorHDR = Color.red;
	}

	public void LongHeads()
    {
        foreach(NonPlayableCharacter npc in Object.FindObjectsOfType<NonPlayableCharacter>())
        {
            npc.skeleton.GetSocketBySlot(Cloth.ESlot.HEAD).bone.localPosition += Vector3.up * Random.Range(.2f, 1f);
        }
    }

    public void MauvaisesTetes()
    {
        foreach(NonPlayableCharacter npc in Object.FindObjectsOfType<NonPlayableCharacter>())
        {
            npc.skeleton.GetSocketBySlot(Cloth.ESlot.HEAD).bone.localScale = Vector3.one * Random.Range(3f, 6f);
            npc.skeleton.GetSocketBySlot(Cloth.ESlot.HEAD).bone.localPosition += Vector3.up * Random.Range(.2f, 1f);
        }
    }

    public void ResetScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void SetAllHunderToHundred()
    {
        foreach(NonPlayableCharacter npc in Object.FindObjectsOfType<NonPlayableCharacter>())
            npc.SetStat(NonPlayableCharacter.EStat.HUNGER, 100f);
    }

    public void SpawnNPC()
    {
        var o = Object.Instantiate(Game.i.library.npcLibrary.NPCSamplePrefab);
        o.transform.position = Game.i.player.GetCurrentController().transform.position + Game.i.player.GetCurrentController().transform.forward * 3f;
    }

    public void SpawnPoof()
    {
        Pyromancer.PlayGameEffect("gfx_step", Game.i.player.GetCurrentController().transform.position);
    }

    public void SpawnFire()
    {
        Pyromancer.PlayVFXAttached("vfx_fire", Game.i.player.GetCurrentController().transform);
    }

    public void Mouette()
    {
	    GameObject _m = GameObject.Find("Seagull");
	    if (_m)
	    {
		    GameObject _mouette = Object.Instantiate(_m, Game.i.player.GetCurrentController().visuals.transform);
		    _mouette.transform.localPosition = new Vector3(0, .2f, 0);
		    _mouette.transform.localEulerAngles = new Vector3(90, 180, 0);
	    }
    }
    
    public void Crab()
    {
	    GameObject _c = GameObject.Find("Crab (1)");
	    if (_c)
	    {
		    GameObject _crab = Object.Instantiate(_c, Game.i.player.GetCurrentController().visuals.transform);
		    _crab.transform.localPosition = new Vector3(0, .2f, 0);
		    _crab.transform.localEulerAngles = new Vector3(0, 180, 0);
	    }
    }

    public void PostProcess()
    {
	    UnityEngine.Rendering.Volume _v = Object.FindObjectOfType<UnityEngine.Rendering.Volume>();
	    VolumeProfile _p = _v.profile;
	    _p = Object.Instantiate(_p);

	    UnityEngine.Experimental.Rendering.HDPipeline.DepthOfField _d;

	    _p.TryGet<UnityEngine.Experimental.Rendering.HDPipeline.DepthOfField>(out _d);
	    if (_d != null)
	    {
		    _d.active = !_d.active;
	    }
    }

    public void ToggleInputs()
    {
	    GameObject _g = GameObject.Find("GAME_CANVAS").transform.GetChild(0).gameObject;
	    _g.SetActive(!_g.activeSelf);
    }
}
