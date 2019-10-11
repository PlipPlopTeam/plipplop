using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    public Library library;
    public Brain player;
    public Mapping mapping;
    [HideInInspector] public Aperture aperture;

    static public Game i;

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

        aperture = FindObjectOfType<Aperture>() ?? Instantiate(library.cameraController).GetComponent<Aperture>();

        i = this;

        mapping = Instantiate<Mapping>(mapping);

        // Init
        player = new Brain(mapping);
    }

    private void Update()
    {
        mapping.Read();
        player.Update();
    }

    private void FixedUpdate()
    {
        player.FixedUpdate();
    }
}
