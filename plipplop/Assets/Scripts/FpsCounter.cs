using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Object = UnityEngine.Object;

public class FpsCounter : MonoBehaviour
{
    public TextMeshProUGUI fpsText;

    private void Start()
    {
        fpsText.gameObject.SetActive(!fpsText.gameObject.activeSelf);
    }

    void Update()
    {
        if (fpsText.gameObject.activeSelf)
        {
            var _objects = FindObjectsOfType(typeof(Object));

            string[] _lastObjects = new string[3]{_objects[0].name, _objects[1].name, _objects[2].name};
            
            string displayFormat = "{0} FPS\n{1} objects\n\nLast objects:\n{2}";
            fpsText.text = string.Format(displayFormat, Mathf.Floor(1 / Time.deltaTime).ToString("n0"), _objects.Length.ToString(), string.Join(",", _lastObjects));
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            fpsText.gameObject.SetActive(!fpsText.gameObject.activeSelf);
        }
    }
}
