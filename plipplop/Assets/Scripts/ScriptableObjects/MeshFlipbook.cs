using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MeshFlipbook", menuName = "ScriptableObjects/MeshFlipbook", order = 1)]
public class MeshFlipbook : ScriptableObject
{
    public string animationName;
    public List<Mesh> meshes;

    public float fps;
}
