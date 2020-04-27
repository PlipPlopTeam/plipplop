using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class EmotionRenderer : MonoBehaviour
{
    [Header("Settings")]
    public Transform headTransform;
    public Vector3 adjustment;
    public float size = 1f;
    public MeshRenderer bubbleBackground;

    EmotionBubble bubble;
    Emotion emotion;
    float timer;
    int frameIndex;
    Emotions library { get { return Game.i.library.emotions; } }

    readonly float speed = 0.5f;
    readonly float duration = 3f;

    // Fired once on awake by the NPCs
    public void Initialize()
    {
        bubble = CreateBoard();
        bubbleBackground.material = Object.Instantiate(bubbleBackground.material);
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
        if(bubble.gameObject.activeSelf && Game.i.aperture != null)
        {
            bubble.transform.forward = -(Game.i.aperture.position.current - bubble.transform.position);
            bubble.transform.position = headTransform.position + adjustment;


            // Animation
            if(emotion != null)
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
        bubble.Set(emotion);
        bubbleBackground.material.mainTexture = library.GetBubbleSprite(newEmotion.bubbleType);

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
    }

    void NextFrame()
    {
        bubble.NextFrames();
    }
}
