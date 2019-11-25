using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PropCounter : MonoBehaviour
{
    string str;
    TextMeshProUGUI txt;

    // Start is called before the first frame update
    void Start()
    {
        txt = GetComponent<TextMeshProUGUI>();
        str = txt.text;
    }

    // Update is called once per frame
    void Update()
    {
        txt.text = string.Format(str, Game.i.chunkLoader.GetPropCount().ToString());
    }
}
