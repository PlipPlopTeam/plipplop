using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Text;
using System.Linq;

public class CameraStats : MonoBehaviour
{
    TextMeshProUGUI txt;

    // Start is called before the first frame update
    void Start()
    {
        txt = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        txt.text = new StringBuilder()
            .AppendLine(string.Format("Time before reset: {0}", Mathf.Round(Game.i.aperture.GetSettings().alignAfter- (Time.time-Game.i.aperture.GetLastCameraInput()))))
            .AppendLine(string.Format("Stack size: {0}", Game.i.aperture.GetStackSize()))
            .AppendLine(string.Format("Static positions: {0}", Game.i.aperture.GetStaticPositionsCount()))
            .AppendLine(string.Format("Transitioning on stack? {0}", Game.i.aperture.IsTransitioningOnStack()))
            .ToString();
    }
}
