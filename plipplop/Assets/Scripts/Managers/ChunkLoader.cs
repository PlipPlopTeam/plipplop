using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

public class ChunkLoader
{
    Dictionary<string, ChunkStreamingZone> chunkZones = new Dictionary<string, ChunkStreamingZone>();
    List<string> loadingChunks = new List<string>();
    ChunkStreamingZone playerChunkZone;

    List<ChunkProp> props = new List<ChunkProp>();


    class Footprint
    {
        class Comparable
        {
            public object p1;
            public object p2;
            public string name;
        }

        readonly Vector3 position;
        readonly Quaternion rotation;
        readonly Vector3 scale;
        readonly string name;
        readonly int childrenCount;
        readonly int componentCount;
        readonly string identifier;

        public Footprint(GameObject obj, string identifier)
        {
            
            name = obj.name;
            List<Transform> children = new List<Transform>();
            for (int i = 0; i < obj.transform.childCount; i++) {
                children.Add(obj.transform.GetChild(i));
            }
            childrenCount = children.Count;
            componentCount = obj.GetComponents(typeof(Component)).Length;
            position = obj.transform.position;
            rotation = obj.transform.rotation;
            scale = obj.transform.localScale;
            this.identifier = identifier;
        }

        public float Compare(Footprint fp)
        {
            var discriminants = new List<Comparable>();
            int same = 0;

            discriminants.Add(new Comparable() { name = "position", p1 = position, p2 = fp.position });
            discriminants.Add(new Comparable() { name = "rotation", p1 = rotation, p2 = fp.rotation });
            discriminants.Add(new Comparable() { name = "scale", p1 = scale, p2 = fp.scale });
            discriminants.Add(new Comparable() { name = "name", p1 = name, p2 = fp.name });
            discriminants.Add(new Comparable() { name = "children", p1 = childrenCount, p2 = fp.childrenCount });
            discriminants.Add(new Comparable() { name = "components", p1 = componentCount, p2 = fp.componentCount });
            discriminants.Add(new Comparable() { name = "identifier", p1 = identifier, p2 = fp.identifier });

            foreach (var d in discriminants) {
                same += d.p1.Equals(d.p2) ? 1 : 0;
                // Debug.Log("COMPARING " + d.name + " => " + d.p1 + "=?=" + d.p2 + " >> " + (d.p1.Equals(d.p2)));
            }

            return ((float)same) / discriminants.Count;
        }

        public bool Equals(Footprint fp)
        {
            return Compare(fp) == 1f;
        }
    }

    class ChunkProp
    {
        public GameObject prop;
        public string chunkIdentifier;
        public readonly Footprint footprint;

        public ChunkProp(GameObject prop, string identifier)
        {
            footprint = new Footprint(prop, identifier);
            this.prop = prop;
            this.chunkIdentifier = identifier;
        }
    }

    public ChunkLoader()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    public void Update()
    {        
        var c = Game.i.player.GetCurrentController();
        if (c != null) {
            var playerCZ = playerChunkZone;
            foreach (var identifier in chunkZones.Keys) {
                var cz = chunkZones[identifier];
                cz.isPlayerInside = false;
                if (cz.IsInsideChunk(c.transform.position)) {
                    cz.isPlayerInside = true;
                    playerCZ = cz;
                    break;
                }
            }
            if (playerCZ != playerChunkZone) {
                // Check what I have to load and unload
                var loadedChunks = chunkZones.Values.ToList().Where(o => { return o.IsLoaded(); });
                var neighbors = playerCZ.neighborhood;
                var chunksToLoad = neighbors.Where(o=> { return !loadedChunks.Contains(o); }).ToList();
                var chunksToUnload = loadedChunks.Where(o => { return !neighbors.Contains(o); }).ToList();

                // Always load player chunk
                chunksToUnload.RemoveAll(o => o.identifier == playerCZ.identifier);
                chunksToLoad.Add(playerCZ);

                // Commit
                foreach (var cz in chunksToLoad) {
                    Load(cz.identifier);
                }
                foreach (var cz in chunksToUnload) {
                    Unload(cz.identifier);
                }

                if (playerChunkZone != null)  playerChunkZone.isPlayerInside = false;
                playerChunkZone = playerCZ;
            }

        }

    }

    public void Register(ChunkStreamingZone csz)
    {
        chunkZones[csz.identifier] = csz;
    }

    void Load(string identifier)
    {
        if (!loadingChunks.Contains(identifier) && !chunkZones[identifier].IsLoaded()) {
            loadingChunks.Add(identifier);
            SceneManager.LoadSceneAsync(chunkZones[identifier].chunk.name, LoadSceneMode.Additive).completed += delegate {
                OnChunkLoaded(identifier);
            };
        }
    }

    void Unload(string identifier)
    {
        var csz = chunkZones[identifier];
        if (csz.IsLoaded()) {
            foreach(var prop in csz.scene.Value.GetRootGameObjects()) {
                if (!csz.GetComponent<Persistent>()) {
                    if (csz.IsInsideChunk(prop.transform.position)) {
                        Object.Destroy(prop);
                        props.RemoveAll(o => o.prop == prop);
                    }
                    else {
                        foreach(var id in chunkZones.Keys) {
                            if (!chunkZones[id].IsLoaded())
                                continue;

                            var newChunk = chunkZones[id];
                            if (newChunk.IsInsideChunk(prop.transform.position)) {
                                SceneManager.MoveGameObjectToScene(prop, newChunk.scene.Value);
                            }
                        }
                    }
                }
            }
            SceneManager.UnloadSceneAsync(csz.scene.Value);
            csz.scene = null;
        }
    }

    void OnChunkLoaded(string identifier)
    {
        loadingChunks.RemoveAll(o => o == identifier);
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (mode != LoadSceneMode.Additive) 
            return;

        foreach(var id in chunkZones.Keys) {
            if (scene.name == chunkZones[id].chunk.name) {
                chunkZones[id].scene = scene;
                foreach (var prop in scene.GetRootGameObjects()) {
                    var chp = new ChunkProp(prop, id); 
                    // Destroy clones (object is still here)
                    if (props.FindAll(o=> o.footprint.Equals(chp.footprint)).Count > 0) {
                        Debug.LogWarning("Destroyed clone of roaming prop: " + prop.name);
                        Object.DestroyImmediate(prop);
                        continue;
                    }
                    props.Add(chp);
                }

                break;
            }
        }
    }
}
