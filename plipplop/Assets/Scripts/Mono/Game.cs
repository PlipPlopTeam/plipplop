using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    public Library library;
    public Brain player;
    public Mapping mapping;

    private void Awake()
    {
        if (!FindObjectOfType<Kickstarter>()) {
            Destroy(gameObject);
            throw new System.Exception("DO NOT put GAME in your scene. Use the kickstarter");
        }
        if (FindObjectsOfType<Game>().Length > 1) {
            Destroy(gameObject);
            throw new System.Exception("!!! DUPLICATE \"GAME\" INSTANCE !!! THIS SHOULD NOT HAPPEN !!!");
        }
    }
}
