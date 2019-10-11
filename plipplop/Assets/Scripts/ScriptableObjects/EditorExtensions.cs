using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class EditorExtensions
{

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
        return;
    }

    public override void OnPossess()
    {
        return;
    }

    internal override void SpecificMove(Vector3 direction)
    {
        base.Move(direction);
    }

    internal override void Start()
    {
        base.Start();
    }

    internal override void Update()
    {
        base.Update();
    }
}
");
        AssetDatabase.ImportAsset(path);
    }
}
