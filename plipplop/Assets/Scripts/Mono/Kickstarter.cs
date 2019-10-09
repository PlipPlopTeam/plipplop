using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kickstarter : MonoBehaviour
{
    public GameObject game;

    private void Awake()
    {
        var g = Instantiate(game);
        g.name = "_GAME";
        DestroyImmediate(gameObject);
    }
}
