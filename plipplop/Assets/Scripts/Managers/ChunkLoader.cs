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

    class ChunkProp
    {
        public GameObject prop;
        public string chunkIdentifier;
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

                foreach(var prop in scene.GetRootGameObjects()) {
                    if (props.FindAll(o=>o.prop == prop).Count > 0){
                        Object.DestroyImmediate(prop); // Destroy clones (object is still here)
                        continue;
                    }
                    props.Add(new ChunkProp() { chunkIdentifier = id, prop = prop });
                }

                break;
            }
        }
    }
}
