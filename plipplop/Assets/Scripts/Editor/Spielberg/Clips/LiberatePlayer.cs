using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spielberg.Clips
{
    public class LiberatePlayer : SpielbergClip
    {
        public SpielbergLiberatePlayerBehaviour behaviour = new SpielbergLiberatePlayerBehaviour();

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
