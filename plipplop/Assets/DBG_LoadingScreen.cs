using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DBG_LoadingScreen : MonoBehaviour
{
    public TextMeshProUGUI text;
    public float averageTime = 2f;

    bool startedLoading = false;

    void Start()
    {
        text.text = "please wait";
        Game.i.player.Paralyze();
    }

    void Update()
    {
        if (Game.i.chunkLoader.IsLoading()) {
            startedLoading = true;
            text.text = "loading...{0}%".Format(Mathf.FloorToInt((Mathf.Clamp((Time.time - averageTime) / averageTime, 0f, 1f) * 100)).ToString());
        }
        else if (startedLoading){
            Destroy(this.gameObject);
            Game.i.player.Deparalyze();
        }
    }
}
