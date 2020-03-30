using UnityEngine;
using UnityEngine.UI;

public class Transitioner : MonoBehaviour
{
	[Header("References")]
	public Image img;
	public Material material;
	public TransitionProfile[] profiles;

	public TransitionProfile profile;
	public float closeDuration;
	public float openDuration;
	public float idleDuration;
	public float timer;
	public bool active;
	public int phase;

	public void Awake()
	{
		if (img == null || material == null) Destroy(gameObject);

		Game.i.onTransitionCalled += (time) => Play(time);
		img.material = Instantiate(material);
		img.material.SetFloat("_Value", 0f);
	}

	[ContextMenu("DEBUG_Transition")]
	public void DEBUG_Transition()
	{
		Play(5f);
	}

	public void FixedUpdate()
	{
		if(active)
		{
			if (timer > 0)
			{
				timer -= Time.deltaTime;

				if (phase == 0)
				{
					img.material.SetFloat("_Value", profile.closeCurve.Evaluate(1 - (timer / closeDuration)));
				}
				else if (phase == 1)
				{
					img.material.SetFloat("_Value", 1f);
				}
				else if (phase == 2)
				{
					img.material.SetFloat("_Value", profile.openCurve.Evaluate(1 - (timer / openDuration)));
				}
			}
			else
			{
				if (phase == 0)
				{
					phase = 1;
					timer = idleDuration;
				}
				else if (phase == 1)
				{
					phase = 2;
					timer = openDuration;
				}
				else if (phase == 2)
				{
					active = false;
				}
			}
		}
	}

	public void Play(float d, string transition = "Default")
	{
		profile = GetTransition(transition);
		if(profile != null)
		{
			float timePool = d;

			closeDuration = Mathf.Clamp(timePool / 2, 0, profile.maxCloseDuration);
			timePool -= closeDuration;
			openDuration = Mathf.Clamp(timePool / 2, 0, profile.maxOpenDuration);
			timePool -= openDuration;
			idleDuration = timePool;

			active = true;
			phase = 0;
			timer = closeDuration;

			img.material.SetFloat("_Value", 0f);
			img.material.SetFloat("_Speed", profile.panningSpeed);
			img.material.SetTexture("_TexturePattern", profile.pattern);
			img.material.SetVector("_TillingAndDirection", new Vector4(
				profile.tilling.x,
				profile.tilling.y,
				profile.panningDirection.x,
				profile.panningDirection.y
			));
			img.material.SetColor("_ColorPattern", profile.patternColor);
			img.material.SetColor("_ColorBackground", profile.backgroundColor);
		}
	}

	public TransitionProfile GetTransition(string name = "Default")
	{
		foreach(TransitionProfile tp in profiles)
		{
			if(tp.name == name) 
				return tp;
		}
		return null;
	}
}

