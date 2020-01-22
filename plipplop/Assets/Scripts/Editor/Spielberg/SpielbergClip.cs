using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class SpielbergClip : PlayableAsset, ITimelineClipAsset
{
    public SpielbergClipBehaviour behaviour = new SpielbergClipBehaviour();

    public ClipCaps clipCaps {
        get { return ClipCaps.None; }
    }

    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {

        var playable = ScriptPlayable<SpielbergClipBehaviour>.Create(graph, behaviour);
        return playable;
    }
}
