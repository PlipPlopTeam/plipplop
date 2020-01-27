using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spielberg.Clips
{
    public class PlayGameEffect : SpielbergClip
    {
        public SpielbergPlayGameEffectClipBehaviour behaviour = new SpielbergPlayGameEffectClipBehaviour();

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
