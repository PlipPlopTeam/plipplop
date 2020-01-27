using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spielberg.Clips
{
    public class PlayMusic : SpielbergClip
    {
        public SpielbergPlayMusicClipBehaviour behaviour = new SpielbergPlayMusicClipBehaviour();

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
