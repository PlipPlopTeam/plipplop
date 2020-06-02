using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Television : Vanilla
{
    [Header("TELEVISION")]
    [Space(5)]
    [Header("References")]
    public Valuable valuable;
    public Renderer screenRenderer;
    //public GameObject fakeLight;
    public Animation anim;
    public ParticleSystem ps;
    [Space(5)]
    [Header("Screen")]
    public Texture standScreenTexture;
    public Texture crouchScreenTexture;
    public Texture idleScreenTexture;

    public override void OnEject()
    {
        base.OnEject();
        screenRenderer.material.mainTexture = idleScreenTexture;
        //fakeLight.SetActive(true);
    }

    internal override void Start()
    {
        base.Start();
        screenRenderer.material = Instantiate(screenRenderer.material);
    }

    void OnDestroy() 
    {
        Destroy(screenRenderer.material);
    }

    internal override void OnLegsRetracted()
    {
        base.OnLegsRetracted();
        
        valuable.hidden = true;
        //fakeLight.SetActive(true);
        screenRenderer.material.mainTexture = crouchScreenTexture;
    }

    internal override void OnLegsExtended()
    {
        base.OnLegsExtended();

        valuable.hidden = false;
        screenRenderer.material.mainTexture = standScreenTexture;
        //fakeLight.SetActive(false);
    }
}
