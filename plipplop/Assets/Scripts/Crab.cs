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

public class Crab : MonoBehaviour
{
    public CrabType type = CrabType.Alone;
    public Crab nextCrab;

    public CrabDance crabDance;
    
    public GameObject knife;
    public Transform knifeTransform;

    private GameObject holdedKnife;
    
    public float knifeChance = .1f;
    private bool hasKnife;
    public bool hidden;

    public Collider col;
    public float hiddenMaxTime = 2f;

    public float respawnRange = 1f;

    
    void Start()
    {
        if (type != CrabType.Alone  && type!=CrabType.First)
        {
            Hide(true);
        }
        
        if (Random.Range(0f, 1f) < knifeChance)
        {
            holdedKnife = Instantiate(knife, knifeTransform);
            hasKnife = true;
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (!hidden)
        {
            if (type == CrabType.Line || type ==CrabType.First)
            {
                if (nextCrab)
                {
                    nextCrab.Show();
                }
                else
                {
                    print("la variable nextCrab n'est pas renseignée");
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

    void Hide(bool _stayHidden = false)
    {
        hidden = true;
        col.enabled = false;
        
        Pyromancer.PlayGameEffect("gfx_sand_poof", transform.position);

        transform.position -= Vector3.up;

        if (!_stayHidden)
        {
//            print("repop soon");
            Invoke("Show", Random.Range(hiddenMaxTime / 2f, hiddenMaxTime));

        }

        if (hasKnife)
        {
            hasKnife = false;
            holdedKnife.SetActive(false);
        }
    }

    public void Pop()
    {
        Pyromancer.PlayGameEffect("gfx_sand_poof", transform.position);
    }


    void Show()
    {
//        RaycastHit hit;
//        if (Physics.Raycast(transform.position, Vector3.up,out hit, 1.1f))
//        {
//            print(hit.collider.gameObject.name);
//            Invoke("Show", Time.deltaTime);
//            return;
//        }
        
    if(holdedKnife !=null)
    {
        if (Random.Range(0f, 1f) < knifeChance)
        {
            holdedKnife.SetActive(true);
            hasKnife = true;
        }
    }
    else
    {
        if (Random.Range(0f, 1f) < knifeChance)
        {
            holdedKnife = Instantiate(knife, knifeTransform);
            hasKnife = true;
        }
    }
        hidden = false;
        col.enabled = true;
        transform.position += Vector3.up;
        transform.position += new Vector3(Random.Range(-respawnRange/2, respawnRange/2),0,Random.Range(-respawnRange/2, respawnRange/2));
        
        Pyromancer.PlayGameEffect("gfx_sand_poof", transform.position);
    }
    
}
