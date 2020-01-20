using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AIPath : MonoBehaviour
{
	[System.Serializable]
	public class Point
	{
		public Vector3 position;
		public float range;
	}

	public bool loop = true;
    public List<Point> points = new List<Point>();

    private void Start()
    {
        Game.i.aiZone.Register(this);
    }

	public Vector3 GetPosition(int id)
	{
		return points[id].position + Geometry.GetRandomPointInRange(points[id].range);
	}
}
