using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlendShapeAnimator : MonoBehaviour
{

    public SkinnedMeshRenderer renderer;
    public int blendShapeAmount;
    public float maxTime;
    public bool randomY;
    public float maxAngleChange;
    
    void Start()
    {
        StartCoroutine(ChangePose());
    }
    
    

    IEnumerator ChangePose()
    {
        float _time = Random.Range(0f, maxTime);

        int _selectedBlendShapeIndex = Random.Range(0, blendShapeAmount);

        if (randomY)
        {
            transform.localEulerAngles += new Vector3(0, Random.Range(-maxAngleChange, maxAngleChange), 0);
        }

        renderer.SetBlendShapeWeight(_selectedBlendShapeIndex,Random.Range(0,100));

        yield return new WaitForSecondsRealtime(_time);
        
        renderer.SetBlendShapeWeight(_selectedBlendShapeIndex,0);
        StartCoroutine(ChangePose());
    }
}
