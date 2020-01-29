using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIZoneDebugger : MonoBehaviour
{
    private void OnDrawGizmosSelected()
    {
        foreach (AIPath path in Game.i.aiZone.GetPaths())
		{
            foreach(AIPath.Point point in path.points)
			{
                Gizmos.DrawWireMesh((Mesh)Resources.Load("Meshes/WireFlag", typeof(Mesh)), point.position, Quaternion.identity, new Vector3(1f, 1f, 1f));
            }
        }
    }
}
