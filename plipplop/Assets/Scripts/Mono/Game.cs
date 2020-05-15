using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Game : MonoBehaviour
{
    public Library library;
    public Pyromancer vfx;
    public Spielberg cinematics;
    public Brain player;
    public AIZone aiZone;
    public Mapping mapping;
    public Switches switches;
    public CheatCodeListener cheatCodeListener;
    public ChunkLoader chunkLoader;
    public QuestMaster quests;
    [HideInInspector] public Aperture aperture;
    [Range(0, -100)] public int killZ = -20;

    public Action<float, float> onTransitionCalled;

    // Dialogs
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
        quests = new QuestMaster();

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

    public void Transition(float closeTime, float waitTime, Action onClosed = null, Action onOpenned = null)
    {
        if (onTransitionCalled != null) onTransitionCalled.Invoke(closeTime, waitTime);
        
        WaitAndDo(closeTime, () => {
            if (onClosed != null) onClosed.Invoke();
            WaitAndDo(waitTime, () => {
                if (onOpenned != null) onOpenned.Invoke();
            });
        });
    }

    public void WaitAndDo(float time, Action then)
    {
        if (then == null) return;
        StartCoroutine(CWaitAndDo(time, then));
    }

    public IEnumerator CWaitAndDo(float time, Action then)
    {
        yield return new WaitForSeconds(time);
        if (then != null) then.Invoke();
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

    public void PlayDialogue(Dialog dial, TalkableCharacter character)
    {
        DialogHooks.dialogToBeGrabbed = dial;
        DialogHooks.currentInterlocutor = character;
    }

}
