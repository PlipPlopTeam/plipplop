using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldBoundariesVolume : Volume
{
    public bool isInvisible = false;
    public GameObject visualBoundariesPrefab;

    private void OnValidate()
    {
        offset = Vector3.up * (height / 2f);
    }
    
    private void Start()
    {
        if (visualBoundariesPrefab != null)
        {
            offset = Vector3.up * (height / 2f);
            var g = Instantiate(visualBoundariesPrefab, this.transform);
            g.transform.localScale = GetSize();
            if (isInvisible)
            {
                foreach (var mr in g.GetComponentsInChildren<MeshRenderer>())
                {
                    mr.enabled = false;
                }
            }
        }
    }

    public override void OnPlayerEnter(Controller player)
    {
        //throw new System.NotImplementedException();
    }

    public override void OnPlayerExit(Controller player)
    {
        //throw new System.NotImplementedException();
    }
}
