using UnityEngine;
using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;

public class Launcher : MonoBehaviour
{
	[Header("References")]
	public CinemachineDollyCart cart;
	public CinemachineSmoothPath path;
	[Header("Settings")]
	public AnimationCurve speedCurve;
	public float speedMultiplier = 1f;
	public string throwGameEffect = "";
	public string releaseGameEffect = "";
	public bool alignWithVelocity = true;
	public float rotationSpeed = 0f;

	[Header("Transition And Timing")]
	public float delay = 0f;
	[Range(0f, 1f)] public float arrivedProgression = 0.75f;
	public float transitionCloseDuration = 1f;
	public float transitionWaitDuration = 1f;

	[Header("Camera")]
	public Transform aim;
	public float FOV = 60f;
	public bool isImmediate = false;
	public bool lookAtTarget = false;

	public Action<GameObject> onArrived;

	private bool throwing = false;
	private bool arrived = false;
	private GameObject thrownObject;
	private float progression = 0.5f;
	private int lookAtIndex = 0;
	private Geometry.PositionAndRotation objective;
	private Vector3 lastPosition;

	public void Update()
	{
		if (throwing) Move();
	}

	public void Move()
	{
		progression += Time.deltaTime * speedMultiplier * speedCurve.Evaluate(progression);
		if (progression < 1f) Frame();

		if(!arrived && progression >= arrivedProgression)
		{
			arrived = true;
			Release();
		}
	}

	public void Frame()
	{
		cart.m_Position = progression * (path.m_Waypoints.Length - 1);
		
		if(thrownObject != null)
		{
			if (alignWithVelocity) thrownObject.transform.Rotate(Vector3.up * rotationSpeed);
			else thrownObject.transform.up = Vector3.up;
		}
		
		lastPosition = cart.gameObject.transform.position;
	}

	public void PrepareLaunch(GameObject obj, Action<GameObject> onArrivedEvent = null)
	{
		cart.m_Position = 0f;
		obj.transform.position = cart.transform.position;
		PlaceCamera();

		if (delay > 0) StartCoroutine(WaitAndLaunch(delay, obj, onArrivedEvent));
		else
		{
			Launch(obj, onArrivedEvent);
		}
	}

	IEnumerator WaitAndLaunch(float time, GameObject obj, Action<GameObject> onArrivedEvent = null)
	{
		yield return new WaitForSeconds(time);
		Launch(obj, onArrivedEvent);
	}

	public void PlaceCamera()
	{
		objective = Game.i.aperture.AddStaticPosition(aim);


		if (isImmediate)
		{
			Game.i.aperture.FixedUpdate();
			Game.i.aperture.fieldOfView.destination = FOV;
			Game.i.aperture.Teleport();
		}

		if (!lookAtTarget)
		{
			lookAtIndex = Game.i.aperture.DisableLookAt();
		}
		else
		{
			Game.i.aperture.EnableLookAt();
		}
	}

	public Vector3 GetThrowDirection()
	{
		if (path.m_Waypoints.Length < 2) return Vector3.up;
		return (path.m_Waypoints[1].position - path.m_Waypoints[0].position).normalized;
	}

	public void ResetCamera()
	{
		Game.i.aperture.RemoveStaticPosition(objective);

		if (isImmediate)
		{
			Game.i.aperture.Unfreeze();
		}

		if (!lookAtTarget)
		{
			Game.i.aperture.RestoreLookAt(lookAtIndex);
		}

		if (releaseGameEffect != "")
		{
			Pyromancer.PlayGameEffect(releaseGameEffect, thrownObject.transform.position);
		}
	}

	public void Launch(GameObject obj, Action<GameObject> onArrivedEvent = null)
	{
		if (obj == thrownObject) return;

		thrownObject = obj;
		throwing = true;
		progression = 0f;
		thrownObject.transform.SetParent(cart.transform);
		thrownObject.transform.localPosition = Vector3.zero;
		thrownObject.transform.localEulerAngles = Vector3.zero;
		thrownObject.transform.up = -cart.gameObject.transform.forward;
		
		Game.i.aperture.Shake(.1f,1,.3f);


		if (onArrivedEvent != null) onArrived += onArrivedEvent;	
		if (throwGameEffect != "") Pyromancer.PlayGameEffect(throwGameEffect, thrownObject.transform.position);
	}

	public void Release()
	{
		Game.i.Transition(transitionCloseDuration, transitionWaitDuration, 
		() => 
		{
			this.ResetCamera();
		}, 
		() =>
		{
			if (this.onArrived != null)
			{
				this.onArrived.Invoke(this.thrownObject);
				this.onArrived = null;
				
				Game.i.aperture.Shake(.05f,.5f,.15f);
			}
			this.arrived = false;
			this.throwing = false;
			this.thrownObject.transform.SetParent(null);
			this.thrownObject = null;
		});
	}

	private void OnTriggerEnter(Collider other)
	{
		if(other.gameObject.TryGetComponent<BaseController>(out BaseController controller))
		{
			if (thrownObject != controller.gameObject) LaunchController(controller);
		}
	}

	public void LaunchController(Controller controller)
	{
		controller.Paralyse();
		controller.Freeze();
		controller.locomotion.StartFly();
		controller.rigidbody.constraints = RigidbodyConstraints.FreezeAll;

		PrepareLaunch(controller.gameObject,
		(go) =>
		{
			controller.UnParalyse();
			controller.UnFreeze();
			controller.locomotion.EndFly();
			controller.rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
			controller.transform.up = Vector3.up;
			Game.i.aperture.EnableLookAt();
		});
	}
}
