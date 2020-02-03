using UnityEngine;
using System.Collections;
using System.Collections.Generic;

class Board
{
    public GameObject obj;
    public MeshRenderer mr;
}

public class EmotionRenderer : MonoBehaviour
{
    [Header("Settings")]
    public List<Emotion> emotions = new List<Emotion>();
    public Transform headTransform;
    public Vector3 adjustment;
    public float size = 1f;

    Board board;
    Emotion emotion;
    float timer;
    int frameIndex;

    public void Load()
    {
        board = CreateBoard();
        Hide();
    }

    Board CreateBoard()
    {
        Board newBoard = new Board();

        newBoard.obj = new GameObject();
        newBoard.obj.name = "EmotionBoard";
        // Mesh Renderer
        newBoard.mr = newBoard.obj.AddComponent<MeshRenderer>();
        newBoard.mr.material = Instantiate(Game.i.library.emotionBoardMaterial);
        // Mesh Filter
        newBoard.obj.AddComponent<MeshFilter>().mesh = Game.i.library.primitiveQuadMesh;
        // Transform adjustment
        newBoard.obj.transform.localScale *= size;

        return newBoard;
    }

    void Update()
    {
        // Face camera
        if(board.obj.activeSelf && Game.i.aperture != null)
        {
            board.obj.transform.forward = -(Game.i.aperture.position.current - board.obj.transform.position);
            board.obj.transform.position = headTransform.position + adjustment;


            // Animation
            if(emotion != null)
            {
                timer += Time.deltaTime;
                if(timer > emotion.speed)
                {
                    NextFrame();
                    timer = 0f;
                }
            }
        }
    }

    public void Show(string emotionName, float duration = 0f)
    {
        emotion = FindEmotion(emotionName);
        if(emotion == null) return;
        timer = 0f;
        frameIndex = 0;
        board.mr.material.SetTexture("_MainTex", emotion.frames[0]);
        board.obj.SetActive(true);

        if(duration > 0f) StartCoroutine(HideAfter(duration));
    }

    public void Show(Emotion newEmotion, float duration = 0f)
    {
        emotion = newEmotion;
        timer = 0f;
        frameIndex = 0;
        board.mr.material.mainTexture = emotion.frames[0];
        board.obj.SetActive(true);

		if (duration > 0f) StartCoroutine(HideAfter(duration));
		else StopAllCoroutines();
    }

    IEnumerator HideAfter(float time)
    {
        yield return new WaitForSeconds(time);
        Hide();
    }

    public void Hide()
    {
        emotion = null;

        if(board != null) board.obj.SetActive(false);
    }

    void NextFrame()
    {
        frameIndex++;
        if(frameIndex >= emotion.frames.Length) frameIndex = 0;

        board.mr.material.mainTexture = emotion.frames[frameIndex];
    }

    Emotion FindEmotion(string name)
    {
        if(emotions.Count == 0) return null;

        foreach(Emotion e in emotions)
        {
            if(e.name == name) return e;
        }
        return null;
    }
}
