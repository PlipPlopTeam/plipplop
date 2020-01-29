using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Timeline;
using UnityEngine;
using UnityEngine.Timeline;

[CustomTimelineEditor(typeof(SpielbergTrack))]
[CanEditMultipleObjects]
public class SpielbergTrackEditor : TrackEditor
{
    Texture2D icon;

    public override TrackDrawOptions GetTrackOptions(TrackAsset track, Object binding)
    {
        icon = Resources.Load<Texture2D>("Editor/Sprites/SPR_D_SpielbergTrack");
        return new TrackDrawOptions() { minimumHeight = 40f, icon = icon, trackColor = Color.white };
    }

    public override void OnTrackChanged(TrackAsset track)
    {
        base.OnTrackChanged(track);
    }
}
