using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.HDPipeline;

[CreateAssetMenu(fileName = "MeshFlipbook", menuName = "ScriptableObjects/MeshFlipbook", order = 1)]
public class MeshFlipbook : ScriptableObject
{
    [System.Serializable]
    public class MeshFrame
    {
        public Mesh mesh;
        public Vector3 position;
        public Vector3 euler;
        public Vector3 scale;
    }
    
    public string animationName;
    public List<MeshFrame> meshes;
    public float fps;
    public bool loop = true;
}
