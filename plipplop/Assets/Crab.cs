using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Crab : MonoBehaviour
{
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
            Hide();
        }
    }

    void Hide()
    {
        hidden = true;
        col.enabled = false;
        
        Pyromancer.PlayGameEffect("gfx_sand_poof", transform.position);

        transform.position -= Vector3.up;
        
        
        Invoke("Show", Random.Range(hiddenMaxTime/2f,hiddenMaxTime));

        if (hasKnife)
        {
            hasKnife = false;
            holdedKnife.SetActive(false);
        }
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
