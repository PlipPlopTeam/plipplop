using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DBG_Teletype : MonoBehaviour
{
    TextMeshProUGUI textMesh;


    private void Awake()
    {
        textMesh = GetComponent<TextMeshProUGUI>();
        textMesh.maxVisibleCharacters = 0;
    }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Teletype_Test());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator Teletype_Test()
    {
        var i = 0;
        while (true)
        {
            textMesh.maxVisibleCharacters++;
            yield return new WaitForSeconds(0.25f);
            i++;
        }
    }
}
