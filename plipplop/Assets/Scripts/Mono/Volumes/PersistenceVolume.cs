using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersistenceVolume : Volume
{
    public ChunkStreamingZone parentChunkStreamingZone;

    public override void OnPlayerEnter(Controller player)
    {
       // throw new System.NotImplementedException();
    }

    public override void OnPlayerExit(Controller player)
    {
       // throw new System.NotImplementedException();
    }

    public override void OnObjectEnter(Collider g)
    {
        if (!Game.i.chunkLoader.IsPersistent(g.gameObject)) {
            Game.i.chunkLoader.Store(g.gameObject);
        }
    }

    public override void OnObjectExit(Collider g)
    {
        if (!Game.i.chunkLoader.IsPersistent(g.gameObject)) {
            Game.i.chunkLoader.RemoveFromStorage(g.gameObject);
        }
    }


}
