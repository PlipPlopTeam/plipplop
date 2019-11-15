using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Carryable
{
    void Carry();
    void Drop();
    float Mass();
    Transform Self();
}
