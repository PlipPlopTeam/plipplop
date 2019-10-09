using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Controller
{
    public abstract void OnEject();
    public abstract void OnJump();
    public abstract void OnToggleCrouch(bool crouching);

    public abstract void Move(Vector3 direction);
    public void Move(float fb, float rl) {
        Move(new Vector3(fb, 0f, rl));
    }
}
