using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Persistent : MonoBehaviour {
    private void Start()
    {
        Game.i.chunkLoader.Register(this);
    }
}
