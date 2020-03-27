using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(DialogPlayer))]
public class DialogEffect : MonoBehaviour
{
    public float rumbleForce = 10f;
    public float waveForce = 10f;
    public float waveShiftBetweenLetters = 2f;

    DialogPlayer player;
    TextMeshProUGUI mesh { get { return player.textMesh; } }
    Dialog.Line line;

    List<Tuple<int, int>> rumbles { get { return player.vertexFXs.ContainsKey("rumble") ? player.vertexFXs["rumble"] : null; } }
    List<Tuple<int, int>> waves { get { return player.vertexFXs.ContainsKey("wave") ? player.vertexFXs["wave"] : null; } }

    Coroutine routine;
    int currentIndex = -1;

    struct VertexAnim
    {
        public float angleRange;
        public float angle;
        public float speed;
        public Vector3[] originalVertices;
    }

    private void Awake()
    {
        player = GetComponent<DialogPlayer>();
        
    }

    private void Update()
    {
        if (!player.isPlaying)
        {
            currentIndex = -1;
            return;
        }

        if (player.lineIndex != currentIndex)
        {
            if (routine != null) StopCoroutine(routine);
            
            currentIndex = player.lineIndex;
            Debug.Log("Animating text");
            routine = StartCoroutine(AnimateText());
        }
    }

    void UpdateVisibility()
    {

        mesh.maxVisibleCharacters = player.charIndex;
    }


    IEnumerator AnimateText()
    {
        TMP_TextInfo textInfo = mesh.textInfo;

        Matrix4x4 matrix;

        // Create an Array which contains pre-computed Angle Ranges and Speeds for a bunch of characters.
        VertexAnim[] vertexAnim = new VertexAnim[1024];
        for (int i = 0; i < 1024; i++)
        {
            vertexAnim[i].angleRange = UnityEngine.Random.Range(10f, 25f);
            vertexAnim[i].speed = UnityEngine.Random.Range(1f, 3f);
        }


        // Cache the vertex data of the text object as the Jitter FX is applied to the original position of the characters.

        Action<bool> animate = (isRandom) =>
        {
            TMP_MeshInfo[] cachedMeshInfo = textInfo.CopyMeshInfoVertexData();
            foreach (var rumble in (isRandom ? rumbles : waves))
            {
                for (int i = rumble.Item1; i <= rumble.Item2; i++)
                {
                    TMP_CharacterInfo charInfo = textInfo.characterInfo[i];

                    // Skip characters that are not visible and thus have no geometry to manipulate.
                    if (!charInfo.isVisible)
                        continue;

                    // Retrieve the pre-computed animation data for the given character.
                    VertexAnim vertAnim = vertexAnim[i];

                    // Get the index of the material used by the current character.
                    int materialIndex = textInfo.characterInfo[i].materialReferenceIndex;

                    // Get the index of the first vertex used by this text element.
                    int vertexIndex = textInfo.characterInfo[i].vertexIndex;

                    // Get the cached vertices of the mesh used by this text element (character or sprite).
                    Vector3[] sourceVertices = vertAnim.originalVertices == null ? cachedMeshInfo[materialIndex].vertices.ToArray() : vertAnim.originalVertices;
                    vertAnim.originalVertices = sourceVertices;

                    // Determine the center point of each character at the baseline.
                    //Vector2 charMidBasline = new Vector2((sourceVertices[vertexIndex + 0].x + sourceVertices[vertexIndex + 2].x) / 2, charInfo.baseLine);
                    // Determine the center point of each character.
                    Vector2 charMidBasline = (sourceVertices[vertexIndex + 0] + sourceVertices[vertexIndex + 2]) / 2;

                    // Need to translate all 4 vertices of each quad to aligned with middle of character / baseline.
                    // This is needed so the matrix TRS is applied at the origin for each character.
                    Vector3 offset = charMidBasline;

                    Vector3[] destinationVertices = textInfo.meshInfo[materialIndex].vertices;

                    destinationVertices[vertexIndex + 0] = sourceVertices[vertexIndex + 0] - offset;
                    destinationVertices[vertexIndex + 1] = sourceVertices[vertexIndex + 1] - offset;
                    destinationVertices[vertexIndex + 2] = sourceVertices[vertexIndex + 2] - offset;
                    destinationVertices[vertexIndex + 3] = sourceVertices[vertexIndex + 3] - offset;

                    vertAnim.angle = Mathf.SmoothStep(-vertAnim.angleRange, vertAnim.angleRange, UnityEngine.Random.value);
                    Vector3 jitterOffset = new Vector3(UnityEngine.Random.Range(-.25f, .25f), UnityEngine.Random.Range(-.25f, .25f), 0);
                    if (!isRandom)
                    {
                        jitterOffset = new Vector3(0f, Mathf.Sin(Time.time + i * waveShiftBetweenLetters), 0f);
                    }

                    matrix = Matrix4x4.TRS(jitterOffset * (isRandom ? rumbleForce : waveForce), Quaternion.Euler(0, 0, 0), Vector3.one);

                    destinationVertices[vertexIndex + 0] = matrix.MultiplyPoint3x4(destinationVertices[vertexIndex + 0]);
                    destinationVertices[vertexIndex + 1] = matrix.MultiplyPoint3x4(destinationVertices[vertexIndex + 1]);
                    destinationVertices[vertexIndex + 2] = matrix.MultiplyPoint3x4(destinationVertices[vertexIndex + 2]);
                    destinationVertices[vertexIndex + 3] = matrix.MultiplyPoint3x4(destinationVertices[vertexIndex + 3]);

                    destinationVertices[vertexIndex + 0] += offset;
                    destinationVertices[vertexIndex + 1] += offset;
                    destinationVertices[vertexIndex + 2] += offset;
                    destinationVertices[vertexIndex + 3] += offset;

                    vertexAnim[i] = vertAnim;
                }

                // Push changes into meshes
                for (int i = 0; i < textInfo.meshInfo.Length; i++)
                {
                    textInfo.meshInfo[i].mesh.vertices = textInfo.meshInfo[i].vertices;
                    mesh.UpdateGeometry(textInfo.meshInfo[i].mesh, i);
                }

            }
        };

        //player.triggerVFX = animate;

        while (true)
        {
            UpdateVisibility();
            mesh.ForceMeshUpdate();

            if (rumbles != null) animate.Invoke(true);
            if (waves != null) animate.Invoke(false);

            yield return new WaitForEndOfFrame();
        }
    }

}
