using UnityEngine;
using Cinemachine;
using System;

public class Launcher : MonoBehaviour
{
	[Header("References")]
	public CinemachineDollyCart cart;
	public CinemachineSmoothPath path;
	[Header("Settings")]
	public AnimationCurve speedCurve;
	public float speedMultiplier = 1f;

	public Action<GameObject> onArrived;

	private bool throwing = false;
	private GameObject thrownObject;
	private float progression = 0.5f;

	public void Update()
	{
		if (throwing) Move();
	}

	public void Move()
	{
		progression += Time.deltaTime * speedMultiplier * speedCurve.Evaluate(progression);
		if (progression < 1f) Frame();
		else Release();
	}

	public void Frame()
	{
		cart.m_Position = progression * (path.m_Waypoints.Length - 1);
	}

	public void Launch(GameObject obj, Action<GameObject> onArrivedEvent = null)
	{
		thrownObject = obj;
		throwing = true;
		progression = 0f;
		thrownObject.transform.SetParent(cart.transform);
		thrownObject.transform.localPosition = Vector3.zero;
		thrownObject.transform.localEulerAngles = Vector3.zero;

		if (onArrivedEvent != null) onArrived += onArrivedEvent;
	}

	public void Release()
	{
		Debug.Log("Released");
		if (onArrived != null)
		{
			onArrived.Invoke(thrownObject);
			onArrived = null;
		}
		throwing = false;
		thrownObject.transform.SetParent(null);
		thrownObject = null;
	}

	private void OnTriggerEnter(Collider other)
	{
		if(other.gameObject.TryGetComponent<BaseController>(out BaseController controller))
		{
			LaunchController(controller);
		}
	}

	public void LaunchController(Controller cont)
	{
		cont.Paralyse();
		cont.Freeze();
		Launch(cont.gameObject, (gameObject) => {
			cont.UnParalyse();
			cont.UnFreeze();
		});
	}
}
