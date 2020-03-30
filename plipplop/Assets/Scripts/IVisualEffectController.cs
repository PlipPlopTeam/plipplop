using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IVisualEffectController
{
    void Activate();
    
    void SetPosition(Vector3 v);

    void SetLocalPosition(Vector3 v);

    void Attach(Transform p);

    void Pause();

     void Destroy();

     void Reset();

     bool IsAlive();

}
