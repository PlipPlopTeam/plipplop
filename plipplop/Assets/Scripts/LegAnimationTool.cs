using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LegAnimationTool : MonoBehaviour
{
    
    public LegAnimator legs;

    public int animationIndex;

    public Transform visuals;


    void Start()
    {
        legs.Play(legs.currentAnimation.animationName);
        
        legs.Attach(visuals);
        visuals.transform.localPosition = Vector3.zero;
        visuals.transform.localEulerAngles = Vector3.zero;
        
        Game.i.player.Eject();
        Destroy(FindObjectOfType<BaseController>().gameObject);
    }

  
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            NextAnimation();
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            PreviousAnimation();
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            ReplayAnimation();
        }
    }

    void NextAnimation()
    {
        animationIndex++;

        if (animationIndex >= legs._animations.Count)
        {
            animationIndex = 0;
        }
        
        legs.Play(legs._animations[animationIndex].animationName);
    }

    void PreviousAnimation()
    {
        animationIndex--;
        
        if (animationIndex <0)
        {
            animationIndex = legs._animations.Count-1;
        }
        legs.Play(legs._animations[animationIndex].animationName);
    }

    void ReplayAnimation()
    {
        legs.Play(legs.currentAnimation.animationName);
    }
}
