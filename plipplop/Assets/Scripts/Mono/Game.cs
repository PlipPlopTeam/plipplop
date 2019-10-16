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

        i = this;

        aperture = new Aperture();
        mapping = Instantiate<Mapping>(mapping);

        SpawnPlayer(Vector3.zero);
    }

    private void SpawnPlayer(Vector3 position)
    {
        // Init
        player = new Brain(mapping);
        Controller c = Instantiate(library.baseControllerPrefab.GetComponent<Controller>(), position, Quaternion.identity);
        player.Possess(c);
        player.SetBaseController(c);
    }

    private void Update()
    {
        mapping.Read();
        player.Update();
        aperture.Update();
    }

    private void FixedUpdate()
    {
        player.FixedUpdate();
        aperture.FixedUpdate();
    }
}
