using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spielberg.Clips
{
    public class StopMusic : SpielbergClip
    {
        public SpielbergStopMusicClipBehaviour behaviour = new SpielbergStopMusicClipBehaviour();

        public override SpielbergClipBehaviour GetBehaviour()
        {
            return behaviour;
        }

        public override string GetDisplayName()
        {
            return behaviour.ToString();
        }
    }
}
