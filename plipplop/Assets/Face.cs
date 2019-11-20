using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Face : MonoBehaviour
{
    [Header("References")]
    public SkinnedMeshRenderer smr;
    [Header("Speak")]
    public bool speaking = false;
    public float speakFrequency = 1f;
    public float speakRandomRange = 1f;
    [Header("Eat")]
    public bool eating = false;
    public float eatFrequency = 1f;
    public float eatRandomRange = 1f;
    [Header("Wink")]
    public bool winking = true;
    public float winkFrequency = 1f;
    public float winkRandomRange = 1f;
    [Header("Happiness")]
    public float happiness = 100f;

    bool eyeClosed;
    bool mouseClosed;
    float speakTimer = 0f;
    float eatTimer = 0f;
    float winkTimer = 0f;

    public void Start()
    {
        if(speaking) Speak();
    }

    public void Set(bool speak, bool eat, bool wink)
    {
        speaking = speak;
        eating = eat;
        winking = wink;
    }

    public void Speak()
    {
        speaking = true;
    }
    public void Shut()
    {
        speaking = false;
        smr.SetBlendShapeWeight(1, 0f);
        smr.SetBlendShapeWeight(2, 0f);
    }

    public void Happiness(float value)
    {
        happiness = Mathf.Clamp(value, 0f, 100f);
        float h = Mathf.Clamp(-100f + happiness * 2f, 0f, 100f);
        float s = Mathf.Clamp(100f - happiness * 2f, 0f, 100f);
        smr.SetBlendShapeWeight(1, h);
        smr.SetBlendShapeWeight(2, s);
    }

    public void Update()
    {
        Happiness(happiness);

        if(speaking)
        {
            if(speakTimer > 0) speakTimer -= Time.deltaTime;
            else
            {
                smr.SetBlendShapeWeight(4, Random.Range(0f, 100f));
                smr.SetBlendShapeWeight(5, Random.Range(0f, 100f));
                speakTimer = speakFrequency + Mathf.Min(0f, Random.Range(-speakRandomRange, speakRandomRange));
            }
        }

        if(winking)
        {
            if(winkTimer > 0) winkTimer -= Time.deltaTime;
            else
            {
                if(!eyeClosed)
                {
                    winkTimer = 0.25f;
                    smr.SetBlendShapeWeight(0, 100f);
                    eyeClosed = true;
                }
                else
                {
                    winkTimer = winkFrequency + Mathf.Min(0f, Random.Range(-winkRandomRange, winkRandomRange));
                    smr.SetBlendShapeWeight(0, 0f);
                    eyeClosed = false;
                }
            }
        }

        if(eating)
        {
            if(eatTimer > 0) eatTimer -= Time.deltaTime;
            else
            {
                eatTimer = eatFrequency + Mathf.Min(0f, Random.Range(-eatRandomRange, eatRandomRange));
                if(!mouseClosed)
                {
                    smr.SetBlendShapeWeight(4, Random.Range(0f, 15f));
                    mouseClosed = true;
                }
                else
                {
                    smr.SetBlendShapeWeight(4, Random.Range(85f, 10f));
                    mouseClosed = false;
                }
            }
        }
    }
}
