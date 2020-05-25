using UnityEngine;

public class Crosshair : MonoBehaviour
{

	[Header("Settings")]
	public float openDistance = 100f;
	public float closedDistance = 50f;
	[Header("References")]
	public RectTransform up;
	public RectTransform right;
	public RectTransform down;
	public RectTransform left;


	private bool shown;
	private bool closed;

	public void Show()
	{
		shown = true;
		up.gameObject.SetActive(true);
		right.gameObject.SetActive(true);
		down.gameObject.SetActive(true);
		left.gameObject.SetActive(true);
		Open();

	}

	public void Hide()
	{
		shown = false;
		up.gameObject.SetActive(false);
		right.gameObject.SetActive(false);
		down.gameObject.SetActive(false);
		left.gameObject.SetActive(false);
	}

	public void Open()
	{
		closed = false;
		up.transform.localPosition = new Vector3(0f, openDistance, 0f);
		down.transform.localPosition = new Vector3(0f, -openDistance, 0f);
		right.transform.localPosition = new Vector3(openDistance, 0f, 0f);
		left.transform.localPosition = new Vector3(-openDistance, 0f, 0f);
	}

	public void Close()
	{
		closed = true;
		up.transform.localPosition = new Vector3(0f, closedDistance, 0f);
		down.transform.localPosition = new Vector3(0f, -closedDistance, 0f);
		right.transform.localPosition = new Vector3(closedDistance, 0f, 0f);
		left.transform.localPosition = new Vector3(-closedDistance, 0f, 0f);
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

					if(s.aim)
					{
						if (!closed) Close();
					}
					else if (closed) Open();
				}
				else if (shown) Hide();
			}
			else if (shown) Hide();
		}
		else if (shown) Hide();
	}
}
