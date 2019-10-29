using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class KeyboardBuffer
{

    //////////////////////////
    ///
    /// Cheats
    /// 

    int bufferLength = 20;
    string keyBuffer = string.Empty;
    Cheats cheats;

    public static KeyCode KeyDown(bool getDef = false)
    {
        KeyCode def = KeyCode.Break;
        if (getDef) return def;
        foreach (KeyCode key in System.Enum.GetValues(typeof(KeyCode))) {
            if ((int)key > 330) break;
            if (Input.GetKeyDown(key)) return key;
        }
        return def;
    }

    public void ListenCheat()
    {
        if (Input.anyKeyDown) {
            var key = KeyDown();
            if (key != KeyDown(true)) {
                keyBuffer += key.ToString();
                while (keyBuffer.Length > bufferLength) keyBuffer = keyBuffer.Remove(0, 1);
                DetectCheat();
            }
        }
    }

    private void DetectCheat()
    {
        foreach (string cheatCode in cheats.Keys) {
            if (keyBuffer.ToUpper().EndsWith(cheatCode)) {
                cheats[cheatCode].Invoke();
                Debug.Log("Cheat detected: "+ cheatCode);
                return;
            }
        }
    }
}
