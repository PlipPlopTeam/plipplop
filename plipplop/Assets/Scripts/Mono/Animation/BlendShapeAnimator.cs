using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlendShapeAnimator : MonoBehaviour
{

    public new SkinnedMeshRenderer renderer;
    public int blendShapeAmount;
    public bool fixedTime = false;
    public float maxTime;
    public bool randomY;
    public float maxAngleChange;

    public bool move = false;
    public float maxMoveRange = .3f;
    
    public LayerMask mask;

    void Start()
    {
        StartCoroutine(ChangePose());
    }
    
    IEnumerator ChangePose()
    {
        float _time = Random.Range(fixedTime?maxTime:0f, maxTime);

        int _selectedBlendShapeIndex = Random.Range(0, blendShapeAmount);

        if (randomY)
        {
            transform.localEulerAngles += new Vector3(0, Random.Range(-maxAngleChange, maxAngleChange), 0);
        }

        if (move)
        {
            transform.position += transform.forward * Random.Range(-maxMoveRange / 2, maxMoveRange / 2);

            RaycastHit hit;
            if(Physics.Raycast(transform.position+new Vector3(0,.1f,0), Vector3.down,  out hit,.2f, mask))
            {
                transform.position = hit.point;
            }
        }
        if(blendShapeAmount!=0)
        renderer.SetBlendShapeWeight(_selectedBlendShapeIndex,Random.Range(0,100));

        yield return new WaitForSecondsRealtime(_time);
        
        renderer.SetBlendShapeWeight(_selectedBlendShapeIndex,0);
        StartCoroutine(ChangePose());
    }
}
