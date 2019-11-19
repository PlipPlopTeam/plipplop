﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using System.Threading.Tasks;

public class ChunkLoader
{
    Dictionary<string, ChunkStreamingZone> chunkZones = new Dictionary<string, ChunkStreamingZone>();
    List<string> loadingChunks = new List<string>();
    ChunkStreamingZone playerChunkZone;

    List<ChunkProp> props = new List<ChunkProp>();

    List<FadedApparition> fadedProps = new List<FadedApparition>();
    List<Persistent> persistentProps = new List<Persistent>();

    List<ChunkProp> disabledProps = new List<ChunkProp>();
    Scene cacheScene;
    readonly float deferringDelay = 0.1f;
    Task currentDeferringTask = null;

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
        public GameObject propObject;
        public string chunkIdentifier;
        public readonly Footprint footprint;
        public bool isFadingOut = false;

        public ChunkProp(GameObject propObject, string identifier)
        {
            footprint = new Footprint(propObject, identifier);
            this.propObject = propObject;
            this.chunkIdentifier = identifier;
        }
    }

    public ChunkLoader()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        if (Game.s.CACHE_CHUNK_PROPS) {
            cacheScene = SceneManager.CreateScene("_CACHE");
        }
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

    public void Register(FadedApparition fa)
    {
        fadedProps.AddUnique(fa);
    }

    public void Register(Persistent p)
    {
        persistentProps.AddUnique(p);
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
            var rootGameObjects = csz.scene.Value.GetRootGameObjects();
            foreach (var prop in props.FindAll(o=>rootGameObjects.Contains(o.propObject))) {
                if (!persistentProps.Find(o=>o.gameObject==csz)){
                    if (csz.IsInsideChunk(prop.propObject.transform.position)) {
                        if (Game.s.CACHE_CHUNK_PROPS) {
                            DisableProp(prop);
                        }
                        else {
                            DestroyProp(prop.propObject);
                        }
                    }
                    else {
                        foreach(var id in chunkZones.Keys) {
                            if (!chunkZones[id].IsLoaded())
                                continue;

                            var newChunk = chunkZones[id];
                            if (newChunk.IsInsideChunk(prop.propObject.transform.position)) {
                                SceneManager.MoveGameObjectToScene(prop.propObject, newChunk.scene.Value);
                            }
                        }
                    }
                }
                else {
                    // If it's persistent, let's move it to the active scene
                    SceneManager.MoveGameObjectToScene(prop.propObject, SceneManager.GetActiveScene());
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

                if (Game.s.DEFERRED_CHUNK_PROPS_LOADING) {
                    // Deferred loading
                    var props = scene.GetRootGameObjects();
                    foreach (var prop in props) {
                        prop.SetActive(false);
                    }

                    if (currentDeferringTask != null) {
                      //  currentDeferringTask
                    }

                    currentDeferringTask = ExecuteListDeferred(props, g => {
                        g.SetActive(true);
                        RegularizeProp(g, id);
                    });
                }
                else {
                    // All at once loading
                    foreach (var prop in scene.GetRootGameObjects()) {
                        RegularizeProp(prop, id);
                    }
                }

                break;
            }
        }
    }

    void RegularizeProp(GameObject prop, string identifier)
    {
        var chp = new ChunkProp(prop, identifier);

        /////////
        // Destroy clones (object is still here)
        var potentialOriginals = props.FindAll(o => o.footprint.Equals(chp.footprint));

        // Getting rid of objects that are "fading away"
        if (Game.s.FADE_CHUNK_PROPS) {
            foreach (var potentialOriginal in potentialOriginals.ToArray()) {
                // an object was "fading out" - let's kill it immediatly instead so we can spawn a new one
                if (potentialOriginal.isFadingOut) {
                    Object.DestroyImmediate(potentialOriginal.propObject);
                    potentialOriginals.Remove(potentialOriginal);
                }
            }
        }

        // If there are still clones, it means this object is a clone, so we shouldn't spawn it
        if (potentialOriginals.Count > 0) {
            Object.DestroyImmediate(prop); // So we just cancel that object
            return;
        }
        //
        /////////


        // Check if object is in cache, if so restore it
        if (IsPropCached(chp.footprint)) {
            chp = GetPropFromCache(chp.footprint);
            EnableProp(chp.footprint);

            // Destroy the "new copy"
            Object.DestroyImmediate(prop);
        }

        props.Add(chp);
    }

    bool IsPropCached(Footprint fp)
    {
        var p = disabledProps.Find(o => o.footprint.Equals(fp));
        return Game.s.CACHE_CHUNK_PROPS  && p != null;
    }

    ChunkProp GetPropFromCache(Footprint fp)
    {
        return disabledProps.Find(o => o.footprint.Equals(fp));
    }

    void EnableProp(Footprint fp)
    {
        var p = disabledProps.Find(o => o.footprint.Equals(fp));
        if (p != null) {
            if (!chunkZones[p.chunkIdentifier].IsLoaded()) return; // Safety in case of asynchronous scene unloading

            disabledProps.Remove(p);
            props.Add(p);
            p.propObject.SetActive(true);
            SceneManager.MoveGameObjectToScene(p.propObject, chunkZones[p.chunkIdentifier].scene.Value);
        }
        else {
            Debug.LogWarning("Tried to enable object with footprint " + fp.GetHashCode() + " but did not find anything to enable (??)");
        }
    }

    void DisableProp(ChunkProp prop, bool destroyImmediatly = false)
    {
        if (disabledProps.Contains(prop)) return;

        // Removing components of objects that don't exist anymore
        // 🤷‍
        fadedProps.RemoveAll(o => o == null);

        var fa = fadedProps.Find(o => { return o.gameObject == prop.propObject; });
        var chunkProp = props.Find(o => { return o.propObject == prop.propObject; });

        SceneManager.MoveGameObjectToScene(prop.propObject, cacheScene);

        if (fa) {
            if (!destroyImmediatly && Game.s.FADE_CHUNK_PROPS) {
                chunkProp.isFadingOut = true;
                fa.FadeOutThen(delegate {
                    props.RemoveAll(o=>ReferenceEquals(o, prop));
                    disabledProps.Add(prop);
                    prop.propObject.SetActive(false);
                });
            }
            else {
                props.RemoveAll(o => ReferenceEquals(o, prop));
                disabledProps.Add(prop);
                prop.propObject.SetActive(false);
            }
        }
        else {
            props.RemoveAll(o => ReferenceEquals(o, prop));
            disabledProps.Add(prop);
            prop.propObject.SetActive(false);
        }
    }

    void DestroyProp(GameObject propObject, bool destroyImmediatly=false)
    {
        // Removing components of objects that don't exist anymore
        // 🤷‍
        fadedProps.RemoveAll(o => o == null);

        var fa = fadedProps.Find(o => { return o.gameObject == propObject; });
        var chunkProp = props.Find(o => { return o.propObject == propObject; });

        if (fa) {
            fadedProps.Remove(fa);
            if (!destroyImmediatly && Game.s.FADE_CHUNK_PROPS) {
                chunkProp.isFadingOut = true;
                SceneManager.MoveGameObjectToScene(propObject, SceneManager.GetActiveScene());
                fa.FadeOutThenDestroy(delegate {
                    props.RemoveAll(o => o.propObject == propObject);
                });
            }
            else {
                props.RemoveAll(o => o.propObject == propObject);
                Object.Destroy(propObject);
            }
        }
        else {
            props.RemoveAll(o => o.propObject == propObject);
            Object.Destroy(propObject);
        }
    }

    Task ExecuteListDeferred<T>(IEnumerable<T> elements, System.Action<T> exe)
    {
        var dt = Time.deltaTime;
        return Task.Run(async delegate {
            foreach (var element in elements) {
                UnityMainThreadDispatcher.Instance().Enqueue(delegate {
                    exe.Invoke(element);
                });
                await Task.Delay(Mathf.RoundToInt(dt * 1000f * deferringDelay));
            }
        });
    }
}
