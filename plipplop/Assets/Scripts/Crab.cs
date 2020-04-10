using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public enum CrabType
{
    Alone,
    First,
    Line,
    Final
}

[RequireComponent(typeof(BlendShapeAnimator))]
public class Crab : MonoBehaviour
{
    public CrabType type = CrabType.Alone;
    public Crab nextCrab;

    public CrabDance crabDance;
    
    public GameObject knife;
    public Transform knifeTransform;

    public float knifeChance = .1f;
    public bool hidden;
    public bool isStatic = false;

    public Collider col;
    public float hiddenMaxTime = 2f;

    public float respawnRange = 1f;

    GameObject heldKnife;
    bool hasKnife;
    BlendShapeAnimator blendShapeAnimator;


    void Start()
    {
        blendShapeAnimator = GetComponent<BlendShapeAnimator>();

        if (type != CrabType.Alone  && type!=CrabType.First)
        {
            Hide(true);
        }
        
        if (!isStatic && Random.Range(0f, 1f) < knifeChance)
        {
            heldKnife = Instantiate(knife, knifeTransform);
            hasKnife = true;
        }

        if (isStatic) {
            blendShapeAnimator.maxMoveRange = 0f;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isStatic && !hidden)
        {
            if (type == CrabType.Line || type ==CrabType.First)
            {
                if (nextCrab)
                {
                    nextCrab.Show();
                }
                else
                {
                    Debug.LogError("la variable nextCrab n'est pas renseignée");
                }
                
                Hide(true);
            }
            else if (type == CrabType.Final)
            {
               // crabDance.StartDancing(false);
                Hide(true);
            }
            else
            {
                if (crabDance)
                {
                    crabDance.StartDancing();
                }

                Hide();
            }
        }
    }

    public void Hide(bool _stayHidden = false)
    {
        hidden = true;
        col.enabled = false;
        
        Pyromancer.PlayGameEffect("gfx_sand_poof", transform.position);

        transform.position -= Vector3.up;

        if (!_stayHidden)
        {
//            print("repop soon");
            Invoke("Show", Random.Range(hiddenMaxTime / 2f, hiddenMaxTime));
            //FIXME: Remove the Invoke Call, use a coroutine
        }

        if (hasKnife)
        {
            hasKnife = false;
            heldKnife.SetActive(false);
        }
    }

    public void Pop()
    {
        Pyromancer.PlayGameEffect("gfx_sand_poof", transform.position);
    }


    public void Show()
    {
        //        RaycastHit hit;
        //        if (Physics.Raycast(transform.position, Vector3.up,out hit, 1.1f))
        //        {
        //            print(hit.collider.gameObject.name);
        //            Invoke("Show", Time.deltaTime);
        //            return;
        //        }

        if (!isStatic) {
            if (heldKnife != null) {
                if (Random.Range(0f, 1f) < knifeChance) {
                    heldKnife.SetActive(true);
                    hasKnife = true;
                }
            }
            else {
                if (Random.Range(0f, 1f) < knifeChance) {
                    heldKnife = Instantiate(knife, knifeTransform);
                    hasKnife = true;
                }
            }
        }

        hidden = false;
        col.enabled = true;
        transform.position += Vector3.up;
        if (!isStatic) transform.position += new Vector3(Random.Range(-respawnRange/2, respawnRange/2),0,Random.Range(-respawnRange/2, respawnRange/2));
        
        Pyromancer.PlayGameEffect("gfx_sand_poof", transform.position);
    }
    
}
