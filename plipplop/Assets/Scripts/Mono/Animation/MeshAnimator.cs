using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshAnimator : MonoBehaviour
{
    public List<MeshFlipbook> _animations;
    
    Dictionary <string, MeshFlipbook> animations = new Dictionary<string, MeshFlipbook>();
    
    public MeshFlipbook currentAnimation;

    public MeshFilter meshFilter;
    
    private Coroutine animCoroutine;
    
    private void Start()
    {
        foreach (var _anim in _animations)
        {
            animations.Add(_anim.animationName, _anim);
        }

        Play(_animations[0].animationName);
    }

    public string GetCurrentAnimation()
    {
        return currentAnimation == null ? null : currentAnimation.animationName;
    }

    public void Play(string _animationName)
    {
        MeshFlipbook _anim = animations[_animationName];

        if (_anim)
        {
            currentAnimation = _anim;
            SetAnimation();
        }
        else
        {
            throw new Exception("animation doesn't exist or the name is incorrect");
        }
    }

    public void PlayOnce(string animationName)
    {
        if (GetCurrentAnimation() != animationName) {
            Play(animationName);
        }
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
        int _frame = 0;
        
        while (true)
        {

            meshFilter.mesh = currentAnimation.meshes[_frame];
            
            yield return new WaitForSeconds(1/currentAnimation.fps);

            _frame++;
            _frame = _frame % currentAnimation.meshes.Count;
        }
    }


}
