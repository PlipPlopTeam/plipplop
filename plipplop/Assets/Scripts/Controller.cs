using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Controller
{
    public abstract void OnEject();
    public abstract void OnJump();
    public abstract void OnToggleCrouch(bool crouching);
    public abstract void Direction(Vector2 direction);
}
