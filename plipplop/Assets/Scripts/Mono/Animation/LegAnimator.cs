using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LegAnimator : MonoBehaviour
{
    [Header("Assets")]
    public List<MeshFlipbook> _animations;
	[Header("Settings")]
	public MeshFlipbook currentAnimation;
    public MeshFilter meshFilter;
    public MeshRenderer meshRenderer;
    public Action onAnimationEnded;
	public Action onNextAnimationEnded;

	Dictionary<string, MeshFlipbook> animations = new Dictionary<string, MeshFlipbook>();
    Coroutine animCoroutine;
    Transform headTransform;
    Transform movementTarget = null;
    Vector3 movementTargetOrigin;
	public float speed = 1f;

    private void Awake()
    {
        headTransform = new GameObject().transform;
        headTransform.SetParent(transform);
        headTransform.localPosition = Vector3.zero;

        foreach (var _anim in _animations)
		{
            if (!animations.ContainsKey(_anim.animationName)) animations.Add(_anim.animationName, _anim);
        }
    }

	public void Reset()
	{
		//speed = 1f;
		timer = 0f;
		frameIndex = 0;
		playing = false;
	}

	public string GetCurrentAnimation()
    {
        return currentAnimation == null ? null : currentAnimation.animationName;
    }
    
    public void PlayOnce(string animationName, Action ended = null)
    {
        if (GetCurrentAnimation() != animationName) Play(animationName, ended);

	}

	public void Play(string animationName, Action ended = null)
	{
        movementTarget = null;
        MeshFlipbook _anim;
        try
		{
            _anim = animations[animationName];
        }
        catch (KeyNotFoundException)
		{
            Debug.LogWarning("!! Animation " + animationName + " doesn't exist in flipbook " + this + " or the name is incorrect");
            Debug.LogWarning("!! List of existing animations: "+string.Join(",", animations.Keys));
            return;
        }
		onNextAnimationEnded = null;
		onNextAnimationEnded += ended;
		currentAnimation = _anim;
		Reset();
		playing = true;
	}

	float timer = 0f;
	int frameIndex = 0;
	bool playing = false;
	void Update()
	{
		if (!playing) return;

		if (timer > 0) timer -= Time.deltaTime;
		else
		{
			Frame(frameIndex);

			if(movementTarget)
			{
				transform.position = Vector3.Lerp(movementTargetOrigin, movementTarget.position, (float)frameIndex / (currentAnimation.meshes.Count - 1));
			}

			frameIndex++;

			if (frameIndex >= currentAnimation.meshes.Count)
			{
				Debug.Log(currentAnimation.name + " has reached end.");
				if (onAnimationEnded != null) onAnimationEnded.Invoke();
				if (onNextAnimationEnded != null)
				{
					onNextAnimationEnded.Invoke();
					onNextAnimationEnded = null;
				}

				if (currentAnimation.loop) frameIndex = 0;
				else playing = false;
			}

			//frameIndex = frameIndex % currentAnimation.meshes.Count;
			timer = (1/currentAnimation.fps) * 1/speed;
		}
	}

	public void Frame(int index)
	{
		if(currentAnimation == null 
		|| index < 0 
		|| index > currentAnimation.meshes.Count) return;

		MeshFlipbook.MeshFrame f = currentAnimation.meshes[index];
		meshFilter.mesh = f.mesh;
		headTransform.localPosition = f.position;
		headTransform.localEulerAngles = f.euler;
		headTransform.localScale = f.scale;
		meshRenderer.sharedMaterial = f.mat;

		if (f.gameEffect != string.Empty)
		{
			Pyromancer.PlayGameEffect(f.gameEffect, transform.position + f.gameEffectOffset);
		}
	}

    public void MoveTo(Transform tr)
    {
        movementTarget = tr;
        movementTargetOrigin = transform.position;
        transform.forward =  (tr.position - transform.position).normalized;
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
