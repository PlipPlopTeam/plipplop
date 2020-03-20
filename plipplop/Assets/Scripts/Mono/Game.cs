using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    [Range(0, -100)] public int killZ = -20;
    public Library library;
    public Pyromancer vfx;
    public Spielberg cinematics;
    public Brain player;
    public AIZone aiZone;
    public Mapping mapping;
    public Switches switches;
    public CheatCodeListener cheatCodeListener;
    public ChunkLoader chunkLoader;
    [HideInInspector] public Aperture aperture;

    static public Game i;
    static public Switches s;

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
        s = switches;

        aperture = new Aperture();
        vfx = new Pyromancer();
        cinematics = new Spielberg();
        mapping = Instantiate<Mapping>(mapping);
        cheatCodeListener = new CheatCodeListener(new Cheats());
        chunkLoader = new ChunkLoader();
        aiZone = new AIZone();

        library.dialogs = new DialogLibrary();
        library.dialogs.Rebuild();  // 🔥

        new GameObject().AddComponent<UnityMainThreadDispatcher>().gameObject.name="_THREAD_DISPATCHER";
        GameObject.Instantiate(library.canvas).name = "GAME_CANVAS";

        var spawn = FindObjectOfType<SpawnMarker>();
        if (!spawn) {
            var g = new GameObject();
            spawn = g.AddComponent<SpawnMarker>();
            g.transform.position = Vector3.up;
        }
        SpawnPlayer(spawn);
        CreateKillZ(spawn);
    }

    private void SpawnPlayer(SpawnMarker spawn)
    {
        // Init
        player = new Brain(mapping);
        Controller c = Instantiate(library.baseControllerPrefab.GetComponent<Controller>(), spawn.position, Quaternion.identity);
        c.transform.forward = spawn.transform.forward;
        player.Possess(c, true);
        player.SetBaseController(c);
    }

    private void Update()
    {
        mapping.Read();
        player.Update();
        aperture.Update();
        chunkLoader.Update();
        cheatCodeListener.ListenCheat();
    }

    private void FixedUpdate()
    {
        player.FixedUpdate();
        aperture.FixedUpdate();
    }

    void CreateKillZ(SpawnMarker spawn)
    {
        var i = Instantiate(library.teleporterVolumePrefab);
        i.name = "_KILLZ";
        i.transform.position = Vector3.up * killZ * 2f;
        var volume = i.GetComponent<TeleportVolume>();
        volume.landingPoint = spawn.transform;
        volume.material = new Material(library.killZMaterial);
        volume.height = Mathf.Abs(killZ);
        volume.width = 4096f;
        volume.length = 4006f;
        volume.isInvisible = true;
    }


}
