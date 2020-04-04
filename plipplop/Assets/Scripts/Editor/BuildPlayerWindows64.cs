
using UnityEditor;
using UnityEngine;
using UnityEditor.Build.Reporting;
using System.IO;
using System;

public class BuildPlayerWindows64 : MonoBehaviour
{
    static readonly string buildLocation = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).FullName, @"builds\current");
    static readonly string shareExportPath = @"D:\BUILDS";
    static readonly string format = @"yyyy.MM.dd_HH'h'mm";

    [MenuItem("Tools/Build diorama")]
    public static void BuildDiorama()
    {
        Build(buildLocation, new[] { "Assets/Scenes/Debug/Diorama.unity" });
    }

    [MenuItem("Tools/Build world scene")]
    public static void BuildWorldScene()
    {
        Build(buildLocation, new[] { "Assets/Scenes/Main.unity" });
    }

    static void Build(string location, params string[] scenes)
    {
        Debug.Log("======================\nSTARTING BUILD...");


        var finalPath = Path.Combine(shareExportPath, DateTime.Now.ToString(format));
        var dirs = new string[] { location, finalPath };
        foreach (var dir in dirs) {
            if (Directory.Exists(dir)) {
                Directory.Delete(dir, true); 
            }
            Directory.CreateDirectory(dir);
        }

        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
        buildPlayerOptions.scenes = scenes;
        buildPlayerOptions.locationPathName = Path.Combine(location, "Build.exe");
        buildPlayerOptions.target = BuildTarget.StandaloneWindows64;
        buildPlayerOptions.options = BuildOptions.None;

        BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
        BuildSummary summary = report.summary;


        if (summary.result == BuildResult.Succeeded) {
            DirectoryCopy(location, Path.Combine(finalPath, "."));
            Debug.Log("Built " + string.Join(", ", scenes) + " to [" + finalPath + "] (" + (summary.totalSize / 1000000f) + " MB)");
        }

        if (summary.result == BuildResult.Failed) {
            if (Directory.Exists(finalPath))  Directory.Delete(finalPath, true);
            Debug.LogError("Build failed");
        }
    }

    static void DirectoryCopy(string SourcePath, string DestinationPath)
    {
        //Now Create all of the directories
        foreach (string dirPath in Directory.GetDirectories(SourcePath, "*",
            SearchOption.AllDirectories))
            Directory.CreateDirectory(dirPath.Replace(SourcePath, DestinationPath));

        //Copy all the files & Replaces any files with the same name
        foreach (string newPath in Directory.GetFiles(SourcePath, "*.*",
            SearchOption.AllDirectories))
            File.Copy(newPath, newPath.Replace(SourcePath, DestinationPath), true);
    }
}
