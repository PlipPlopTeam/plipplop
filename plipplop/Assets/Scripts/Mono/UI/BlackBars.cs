using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlipPlop.UserInterface
{
    public class BlackBars : MonoBehaviour
    {
        public RectTransform blackBars;

        public float speed = 200f;

        float outOfScreenThreshold = -200f;
        float currentPresence;

        private void Start()
        {
            currentPresence = outOfScreenThreshold;
        }

        void Update()
        {
            if (ShouldActivate()) {
                if (currentPresence * Mathf.Sign(outOfScreenThreshold) >= 0f) {
                    currentPresence -= Time.deltaTime * speed * Mathf.Sign(outOfScreenThreshold);
                }
            }
            else {
                if (Mathf.Abs(currentPresence) < Mathf.Abs(outOfScreenThreshold)) {
                    currentPresence += Time.deltaTime * speed * Mathf.Sign(outOfScreenThreshold);
                }
            }
            SetBarPresence(Mathf.Clamp(currentPresence, outOfScreenThreshold, 0f));
        }

        void SetBarPresence(float value)
        {
            blackBars.offsetMin = new Vector2(blackBars.offsetMin.x, value);
            blackBars.offsetMax = new Vector2(blackBars.offsetMax.x, -value);
        }

        bool ShouldActivate()
        {
            return Game.i.player.IsParalyzed() || Game.i.aperture.IsCameraBeingRepositioned() || Game.i.aperture.IsUserAligning();
        }
    }
}