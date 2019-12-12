using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CopyCameraFOV : MonoBehaviour
{
	public Camera target;
	public Camera model;

    void Update()
    {
		if(target && model)
		{
			target.fieldOfView = model.fieldOfView;
		}
	}
}
