using UnityEngine;

public class Crosshair : MonoBehaviour
{

	[Header("Settings")]
	public float minGap = 40f;
	public float maxGap = 70f;
	[Header("References")]
	public RectTransform up;
	public RectTransform right;
	public RectTransform down;
	public RectTransform left;


	private bool shown;

	public void Show()
	{
		shown = true;
		up.gameObject.SetActive(true);
		right.gameObject.SetActive(true);
		down.gameObject.SetActive(true);
		left.gameObject.SetActive(true);
		Gap(maxGap);
	}

	public void Hide()
	{
		shown = false;
		up.gameObject.SetActive(false);
		right.gameObject.SetActive(false);
		down.gameObject.SetActive(false);
		left.gameObject.SetActive(false);
	}

	public void Gap(float d)
	{
		up.transform.localPosition = new Vector3(0f, d, 0f);
		down.transform.localPosition = new Vector3(0f, -d, 0f);
		right.transform.localPosition = new Vector3(d, 0f, 0f);
		left.transform.localPosition = new Vector3(-d, 0f, 0f);
	}

	void Update()
	{
		if (Game.i.player != null && Game.i.player.GetCurrentController() != null)
		{
			if (Game.i.player.GetCurrentController().TryGetComponent<Shooter>(out Shooter s))
			{
				if (!s.AreLegsRetracted())
				{
					if (!shown) Show();
					else Gap(minGap + ((1f - s.chargePercentage) * (maxGap - minGap)));
				}
				else if (shown) Hide();
			}
			else if (shown) Hide();
		}
		else if (shown) Hide();
	}
}
