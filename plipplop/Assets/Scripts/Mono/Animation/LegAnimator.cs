using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LegAnimator : MonoBehaviour
{
    
    public List<MeshFlipbook> _animations;
    public MeshFlipbook currentAnimation;
    public MeshFilter meshFilter;

    Dictionary <string, MeshFlipbook> animations = new Dictionary<string, MeshFlipbook>();
    Coroutine animCoroutine;
    Transform headTransform;

    private void Awake()
    {
        headTransform = new GameObject().transform;
        headTransform.SetParent(transform);
        headTransform.localPosition = Vector3.zero;
    }

    private void Start()
    {        
        foreach (var _anim in _animations)
        {
            if(!animations.ContainsKey(_anim.animationName)) animations.Add(_anim.animationName, _anim);
        }
        Play(_animations[0].animationName);
    }

    public string GetCurrentAnimation()
    {
        return currentAnimation == null ? null : currentAnimation.animationName;
    }
    
    public void PlayOnce(string animationName)
    {
        if (GetCurrentAnimation() != animationName) {
            Play(animationName);
        }
    }
    
    public void Play(string animationName)
    {
        MeshFlipbook _anim;
        try {
            _anim = animations[animationName];
        }
        catch (KeyNotFoundException) {
            Debug.LogWarning("!! Animation " + animationName + " doesn't exist in flipbook " + this + " or the name is incorrect");
            Debug.LogWarning("!! List of existing animations: "+string.Join(",", animations.Keys));
            return;
        }

        currentAnimation = _anim;
        SetAnimation();
    }
    
    void SetAnimation( )
    {
        if (animCoroutine != null)
        {
            StopCoroutine(animCoroutine);
        }
        animCoroutine = StartCoroutine(AnimationPlaying());
    }
    
    private IEnumerator AnimationPlaying()
    {
        int _frameIndex = 0;
        while (true)
        {
            MeshFlipbook.MeshFrame _frame = currentAnimation.meshes[_frameIndex];
            
            meshFilter.mesh = _frame.mesh;
            headTransform.localPosition = _frame.position;
            headTransform.localEulerAngles = _frame.euler;
            headTransform.localScale = _frame.scale;
            
            yield return new WaitForSeconds(1/currentAnimation.fps);
            _frameIndex++;
            
            if (_frameIndex >= currentAnimation.meshes.Count && !currentAnimation.loop)
            {
                yield break;
            }
            _frameIndex = _frameIndex % currentAnimation.meshes.Count;
        }
    }

    public void Attach(Transform head)
    {
        head.SetParent(headTransform);
    }

    public void Detach(Transform head)
    {
        head.parent = null;
    }
}
