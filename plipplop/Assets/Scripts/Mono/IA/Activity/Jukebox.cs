using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jukebox : Activity
{
    public override void Update()
    {
        base.Update();

        foreach(NonPlayableCharacter user in users)
        {
            user.transform.Rotate(user.transform.up * Time.deltaTime * 250f);
        }
    }
}
