using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICarryable
{
    void Carry();
	void Drop();
	bool IsCarried();
	float Mass();
    Transform Self();
}
