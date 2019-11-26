using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AIPath : MonoBehaviour
{
    public bool loop = true;
    public List<Vector3> points = new List<Vector3>();
    [HideInInspector] public int index = 0;

    private void Start()
    {
        Game.i.aiZone.Register(this);
    }
}
