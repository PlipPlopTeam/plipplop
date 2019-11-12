using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlickeringLight : MonoBehaviour
{
    public Light l;
    public Vector2 range;

    // Update is called once per frame
    void Update()
    {
        l.intensity = Random.Range(range.x, range.y);
    }
}
