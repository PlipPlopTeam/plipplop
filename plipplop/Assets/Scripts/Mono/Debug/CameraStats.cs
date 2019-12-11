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
            .ToString();
    }
}
