using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class EmotionRenderer : MonoBehaviour
{
    [Header("Settings")]
    public Transform headTransform;
    public float vOffset = 0f;
    public float hOffset = 0f;

    EmotionBubble bubble;
    Emotion emotion;
    float timer;
    int frameIndex;
    Emotions library { get { return Game.i.library.emotions; } }
    bool isDisplayingEmotion { get { return bubble.gameObject.activeSelf; } }

    readonly float speed = 0.5f;
    readonly float duration = 3f;

    // Fired once on awake by the NPCs
    public void Initialize()
    {
        bubble = CreateBoard();
        Hide();
    }

    EmotionBubble CreateBoard()
    {
        EmotionBubble bubble = GameObject.Instantiate(Game.i.library.bubblePrefab).GetComponent<EmotionBubble>();
        bubble.gameObject.name = "EmotionBoard for "+gameObject.name;
        return bubble;
    }

    void Update()
    {
        // Face camera
        if(isDisplayingEmotion && Game.i.aperture != null)
        {
            bubble.transform.forward = -(Game.i.aperture.position.current - bubble.transform.position);
            bubble.transform.position = headTransform.position + transform.forward * hOffset +Vector3.up*vOffset;
            bubble.isLeftSide = Game.i.aperture.GetCameraTransform().InverseTransformPoint(bubble.transform.position).x < Game.i.aperture.GetCameraTransform().InverseTransformPoint(headTransform.position).x;

            // Animation
            if (emotion != null)
            {
                timer += Time.deltaTime;
                if(timer > speed)
                {
                    NextFrame();
                    timer = 0f;
                }
            }
        }
    }

    public void Show(Emotion.EVerb verbId, string subjectName, Emotion.EBubble bubble = Emotion.EBubble.THINK)
    {
        Show(verbId, new string[] { subjectName }, bubble);
    }

    public void Show(Emotion.EVerb verbId, string[] subjectNames, Emotion.EBubble bubble=Emotion.EBubble.THINK)
    {
        var verb = library.GetVerb(verbId);
        var subjects = subjectNames.Select(o => library.GetSubject(o));
            
        emotion = new Emotion(bubble, verb, subjects.ToArray());

        Show(emotion);
    }

    public void Show(Emotion newEmotion)
    {
        emotion = newEmotion;
        timer = 0f;
        frameIndex = 0;
        bubble.gameObject.SetActive(true);
        bubble.Set(emotion);
        bubble.animator.SetTrigger("popUp");
        StartCoroutine(HideAfter(duration));
    }

    IEnumerator HideAfter(float time)
    {
        yield return new WaitForSeconds(time);
        Hide();
    }

    public void Hide()
    {
        emotion = null;
        bubble.VoidRenderers();
        bubble.gameObject.SetActive(false);
    }

    void NextFrame()
    {
        bubble.NextFrames();
    }
}
