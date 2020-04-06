using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogPrompt : MonoBehaviour
{
    public float markerHeight = 2f;
    public Image promptLogo;

    // Start is called before the first frame update
    void Start()
    {
        promptLogo.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Game.i.player.currentChatOpportunity != null && !Game.i.player.IsParalyzed()) {
            promptLogo.enabled = true;
            promptLogo.rectTransform.position = Camera.main.WorldToScreenPoint(Game.i.player.currentChatOpportunity.transform.position + Vector3.up*markerHeight);
        }
        else {
            promptLogo.enabled = false;
        }
    }
}
