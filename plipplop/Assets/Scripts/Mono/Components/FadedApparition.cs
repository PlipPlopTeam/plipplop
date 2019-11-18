using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadedApparition : MonoBehaviour
{
    Renderer[] renderers;
    float alpha = 0f;
    float speed = 1f;

    private void Awake()
    {
        renderers = GetComponentsInChildren<Renderer>();
        UpdateColor();
    }

    private void OnEnable()
    {
        StartCoroutine(FadeIn());
    }

    private void OnDisable()
    {
        alpha = 0f;
    }

    public void StartFadingOut()
    {
        StartCoroutine(FadeOut());
    }

    public void StartFadeIn()
    {
        StartCoroutine(FadeIn());
    }

    IEnumerator FadeIn()
    {
        while(alpha < 1f) {
            alpha += Time.deltaTime * speed;
            UpdateColor();
            yield return null;
        }
        yield return true;
    }

    IEnumerator FadeOut()
    {
        while (alpha > 0f) {
            alpha -= Time.deltaTime * speed;
            UpdateColor();
            yield return null;
        }
        yield return true;
    }

    void UpdateColor()
    {
        foreach (var renderer in renderers) {
            renderer.material.SetFloat(Shader.PropertyToID("_FadeAmount"), 1f-alpha);
        }
    }
}
