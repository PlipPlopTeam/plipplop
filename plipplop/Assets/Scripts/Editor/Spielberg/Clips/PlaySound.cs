using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spielberg.Clips
{
    public class PlaySound : SpielbergClip
    {
        public SpielbergPlaySoundClipBehaviour behaviour = new SpielbergPlaySoundClipBehaviour();

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
