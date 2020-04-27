using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EmotionBubble : MonoBehaviour
{
    public MeshRenderer verbRenderer;
    public MeshRenderer singleSubjectRenderer;
    public MeshRenderer[] smallSubjectsRenderers;

    List<TextureRoll> textureRolls = new List<TextureRoll>();

    public class TextureRoll
    {
        public int currentFrame = 0;
        public Texture[] frames;
        public Material material;

        public TextureRoll(Texture[] frames, Material material)
        {
            this.frames = frames;
            this.material = material;
        }

        public void Next()
        {
            currentFrame = (currentFrame + 1) % frames.Length;
            material.mainTexture = frames[currentFrame];
        }
    }

    private void Awake()
    {
        // Instantiate the materials of everyone 
        verbRenderer.material = Instantiate(verbRenderer.material);
        singleSubjectRenderer.material = Instantiate(singleSubjectRenderer.material);
        foreach (var ren in smallSubjectsRenderers) {
            ren.material = Instantiate(verbRenderer.material);
        }
    }

    public void Set(Emotion emotion)
    {
        textureRolls.Clear();
        VoidRenderers();

        try {
            verbRenderer.material.mainTexture = emotion.verb.frames[0];
            verbRenderer.enabled = true;
            textureRolls.Add(new TextureRoll(emotion.verb.frames, verbRenderer.material));
        }
        catch (System.IndexOutOfRangeException) {
            Debug.LogError("!! The verb " + emotion.verb + " HAS NO FRAMES! Check the scriptable object in the library.");
        }

        if (emotion.subjects.Count == 1) {
            singleSubjectRenderer.material.mainTexture = emotion.subjects[0].frames[0];
            singleSubjectRenderer.enabled = true;
            textureRolls.Add(new TextureRoll(emotion.subjects[0].frames, singleSubjectRenderer.material));
        }
        else if (emotion.subjects.Count == 0) {
            throw new System.Exception("!! NO SUBJECTS in given emotion " + emotion + ". This should NOT happen. Check who triggered that emotion.");
        }
        else {
            try {
                for (int i = 0; i < smallSubjectsRenderers.Length; i++) {
                    smallSubjectsRenderers[i].material.mainTexture = emotion.subjects[i].frames[0];
                    smallSubjectsRenderers[i].enabled = true;
                    textureRolls.Add(new TextureRoll(emotion.subjects[i].frames, smallSubjectsRenderers[i].material));
                }
            }
            catch (System.IndexOutOfRangeException) {
                Debug.LogError("!! One of the subjects " + string.Join(", ", emotion.subjects.Select(o=> { return o.name; })) + " HAS NO FRAMES! Check the scriptable objects.");
            }
        }
    }

    public void VoidRenderers()
    {
        verbRenderer.enabled = false;
        singleSubjectRenderer.enabled = false;
        foreach(var ren in smallSubjectsRenderers) {
            ren.enabled = false;
        }
    }

    public void NextFrames()
    {
        foreach(var roll in textureRolls) {
            roll.Next();
        }
    }
}
