using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(UnityEngine.UI.MaskableGraphic))]
public class BlinkMaskableGraphic : MonoBehaviour
{
    UnityEngine.UI.MaskableGraphic image;

    private void Start()
    {
        image = GetComponent<UnityEngine.UI.MaskableGraphic>();
    }

    // Update is called once per frame
    void Update()
    {
        image.color = new Color(1f, 1f, 1f, Mathf.RoundToInt(Time.time*5) % 2);
    }
}
