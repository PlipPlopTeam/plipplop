using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputDisplay : MonoBehaviour
{
    public List<GameObject> inputsImages;
    
    void Start()
    {
        foreach (var _image in inputsImages)
        {
            _image.SetActive(false);
        }
        
        inputsImages[0].SetActive(true);
    }

    public void ShowShooterInputs()
    {
        inputsImages[0].SetActive(false);
        inputsImages[1].SetActive(true);
    }

    public void ShowVanillaInputs()
    {
        inputsImages[1].SetActive(false);
        inputsImages[0].SetActive(true);
    }

    public void HideInputs()
    {
        foreach (var _image in inputsImages)
        {
            _image.SetActive(false);
        }
    }
}
