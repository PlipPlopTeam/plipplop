using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FpsCounter : MonoBehaviour
{
    public TextMeshProUGUI fpsText;
    
    void Update()
    {
        if (fpsText.gameObject.activeSelf)
        {
            var _objects = FindObjectsOfType(typeof(Object));
            fpsText.text = Mathf.Floor(1 / Time.deltaTime).ToString() + "\n" + _objects.Length + "\n" + _objects[0].name + "\n" + _objects[1].name + "\n" + _objects[2].name;
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            fpsText.gameObject.SetActive(!fpsText.gameObject.activeSelf);
        }
    }
}
