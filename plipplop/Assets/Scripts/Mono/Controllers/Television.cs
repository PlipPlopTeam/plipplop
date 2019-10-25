using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Television : Hopper
{
    [Header("TELEVISION")]
    [Space(5)]
    [Header("References")]
    public Valuable valuable;
    public Renderer screenRenderer;
    public GameObject fakeLight;
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
        fakeLight.SetActive(true);
    }

    internal override void Push(Vector3 direction)
    {
        base.Push(direction);
        anim.Play();
    }

    internal override void Start()
    {
        base.Start();
        screenRenderer.material = Instantiate(screenRenderer.material);
    }

    internal override void OnJumpUp()
    {
        valuable.hidden = true;
    }

    internal override void OnJumpDown()
    {
        ps.Play();
        valuable.hidden = false;
    }

    void OnDestroy() 
    {
        Destroy(screenRenderer.material);
    }

    internal override void OnLegsRetracted()
    {
        base.OnLegsRetracted();
        
        valuable.hidden = true;
        fakeLight.SetActive(true);
        screenRenderer.material.mainTexture = crouchScreenTexture;
    }

    internal override void OnLegsExtended()
    {
        base.OnLegsExtended();

        valuable.hidden = false;
        screenRenderer.material.mainTexture = standScreenTexture;
        fakeLight.SetActive(false);
    }
}
