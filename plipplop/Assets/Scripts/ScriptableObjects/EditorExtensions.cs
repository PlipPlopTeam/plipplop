using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class EditorExtensions
{
#if UNITY_EDITOR

    [MenuItem("Assets/Create/New Controller", priority = 1)]
    static void CreateNewController()
    {
        var path = 
            Path.Combine(
                AssetDatabase.GetAssetPath(Selection.activeObject.GetInstanceID()),
                "NewController.cs"
            );

        File.WriteAllText(path, @"
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class NewController : Controller
{
    public override void OnEject()
    {
        base.OnEject();
        // Code here
    }

    public override void OnPossess()
    {
        base.OnPossess();
        // Code here
    }

    internal override void SpecificMove(Vector3 direction)
    {

    }

    internal override void Start()
    {
        base.Start();
        // Code here
    }

    internal override void Update()
    {
        base.Update();
        // Code here
    }

    internal override void OnLegsRetracted()
    {
        // Code here
    }

    internal override void OnLegsExtended()
    {
        // Code here
    }
}
");
        AssetDatabase.ImportAsset(path);
    }
#endif
}
