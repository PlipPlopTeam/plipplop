using UnityEngine;

[System.Serializable]
public class Emotion
{
    public string name;
    public Texture[] frames;
    public float speed;
}

class Board
{
    public GameObject obj;
    public MeshRenderer mr;
}

public class EmotionRenderer : MonoBehaviour
{
    [Header("Settings")]
    public Emotion[] emotions;
    public Transform headTransform;
    public Vector3 adjustment;
    public float size = 1f;

    Board board;
    Emotion emotion;
    float timer;
    int frameIndex;

    void Start()
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
        //newBoard.mr.material = Instantiate(Library.instance.boardMaterial);
        // Mesh Filter
        //newBoard.obj.AddComponent<MeshFilter>().mesh = Library.instance.primitiveQuadMesh;
        // Transform adjustment
        newBoard.obj.transform.localScale *= size;

        return newBoard;
    }

    void Update()
    {
        // Face camera
        board.obj.transform.forward = -(Camera.main.transform.position - board.obj.transform.position);
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

    public void Show(string emotionName)
    {
        emotion = FindEmotion(emotionName);
        if(emotion == null) return;

        timer = 0f;
        frameIndex = 0;
        board.mr.material.mainTexture = emotion.frames[0];
        board.obj.SetActive(true);
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
        if(emotions.Length == 0) return null;

        foreach(Emotion e in emotions)
        {
            if(e.name == name) return e;
        }
        return null;
    }
}
