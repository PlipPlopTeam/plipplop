using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadedApparition : MonoBehaviour
{
    Renderer[] renderers;
    float alpha = 0f;
    float speed = 1f;
    Coroutine coro;

    private void Awake()
    {
        renderers = GetComponentsInChildren<Renderer>();
        UpdateColor();
    }

    private void Start()
    {
        Game.i.chunkLoader.Register(this);
    }

    private void OnEnable()
    {
        StartFadeIn();
    }

    private void OnDisable()
    {
        alpha = 0f;
    }

    public void StartFadingOut()
    {
        if (coro != null) StopCoroutine(coro);
        coro = StartCoroutine(FadeOut());
    }

    public void StartFadeIn()
    {
        if (coro != null) StopCoroutine(coro);
        coro = StartCoroutine(FadeIn());
    }

    public void FadeOutThenDestroy(System.Action callback=null)
    {
        if (coro != null) StopCoroutine(coro);
        coro = StartCoroutine(FadeOutDestroy(callback));
    }

    IEnumerator FadeIn(System.Action callback = null)
    {
        while(alpha < 1f) {
            alpha += Time.deltaTime * speed;
            UpdateColor();
            yield return null;
        }
        if (callback != null) callback.Invoke();
        yield return true;
    }

    IEnumerator FadeOut(System.Action callback = null)
    {
        while (alpha > 0f) {
            alpha -= Time.deltaTime * speed;
            UpdateColor();
            yield return null;
        }
        if (callback != null) callback.Invoke();
        yield return true;
    }

    IEnumerator FadeOutDestroy(System.Action callback = null)
    {
        yield return FadeOut();
        if (callback != null) callback.Invoke();
        Destroy(this.gameObject);
    }

    void UpdateColor()
    {
        foreach (var renderer in renderers) {
            renderer.material.SetFloat(Shader.PropertyToID("_FadeAmount"), 1f - alpha);
        }
    }
}
