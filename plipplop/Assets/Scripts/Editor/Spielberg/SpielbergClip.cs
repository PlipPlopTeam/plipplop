using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public abstract class SpielbergClip : PlayableAsset, ITimelineClipAsset
{
    public ClipCaps clipCaps {
        get { return ClipCaps.None; }
    }

    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<SpielbergClipBehaviour>.Create(graph, GetBehaviour());
        return playable;
    }

    public abstract SpielbergClipBehaviour GetBehaviour();
    public abstract string GetDisplayName();
}
